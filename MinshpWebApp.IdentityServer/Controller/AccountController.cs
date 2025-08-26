using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Request;
using MinshpWebApp.IdentityServer.Authentication;
using MinshpWebApp.IdentityServer.Helper;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace MinshpWebApp.IdentityServer.Controller;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IHttpClientFactory _http;

    public AccountController(UserManager<AppUser> um, SignInManager<AppUser> sm, IHttpClientFactory http, ILogger<AccountController> logger)
    {
        _userManager = um; _signInManager = sm;
        _http = http;
        _logger = logger;
    }

    public sealed class RegisterDto
    {
        [Required, EmailAddress] public string Email { get; set; } = default!;
        [Required, MinLength(6)] public string Password { get; set; } = default!;
        // infos “métier” à propager si tu veux :
        [Required] public string FirstName { get; set; } = default!;
        [Required] public string LastName { get; set; } = default!;
        [Required] public string Civility { get; set; } = default!;
        [Required] public string? Birthdate { get; set; } = default!;
        public string? Phone { get; set; }
    }

    public sealed class UpdateUserDto
    {
        [Required] public string Id { get; set; } = default!;
        [Required] public string FirstName { get; set; } = default!;
        [Required] public string LastName { get; set; } = default!;
        [Required] public string Civility { get; set; } = default!; // "M" | "Mme"
        [Required] public bool Actif { get; set; } = default!;
        public string? Phone { get; set; }
        public string? Birthdate { get; set; } // "yyyy-MM-dd"
         public string? Pseudo { get; set; } = default!;
        [EmailAddress] public string? Email { get; set; }
    }

    public sealed class ChangePasswordDto
    {
        // Variante 1 (self‑service) : demande l’ancien mot de passe
        public string? CurrentPassword { get; set; }

        // Variante 2 (admin reset) : pas d’ancien mdp
        [Required, MinLength(6)] public string NewPassword { get; set; } = default!;
    }

    public record LoginDto(string Email, string Password);

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var email = dto.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { error = "Email requis." });

        // 1) Contrôle d'existence par email
        var existingByEmail = await _userManager.FindByEmailAsync(email);
        if (existingByEmail != null)
            return Conflict(new { error = "Cet email est déjà enregistré." }); // 409

        // 2) (Redondant ici mais safe) contrôle d'existence par username
        var existingByName = await _userManager.FindByNameAsync(email);
        if (existingByName != null)
            return Conflict(new { error = "Un utilisateur avec ce nom existe déjà." });

        // 3) Création
        var user = new AppUser { UserName = email, Email = email, EmailConfirmed = true };
        var res = await _userManager.CreateAsync(user, dto.Password);
        if (!res.Succeeded)
            return BadRequest(new { errors = res.Errors.Select(e => e.Description) });

        var userId = await _userManager.GetUserIdAsync(user); // = user.Id

        await _userManager.AddClaimsAsync(user, new[]
        {
        new Claim(OpenIddictConstants.Claims.Subject, userId),
        new Claim(OpenIddictConstants.Claims.Email, email),
        new Claim(ClaimTypes.GivenName, dto.FirstName ?? string.Empty),
        new Claim(ClaimTypes.Surname, dto.LastName ?? string.Empty),
    });

        // 3) Appelle ton API pour créer le Customer
        //    3.1 Récupère un token client_credentials
        var apiToken = await GetClientCredentialsTokenAsync(ct);
        if (string.IsNullOrEmpty(apiToken))
        {
            // tu peux logguer + ignorer (création côté API fera plus tard),
            // ou retourner 500 si tu veux rendre l’opération atomique
            return StatusCode(500, "Impossible d’obtenir un jeton technique pour l’API.");
        }


        //    3.2 POST /customer
        var api = _http.CreateClient("api");
        api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);

        // 4) Appel API métier (client)
        var customer = new CustomerRequest
        {
            Civilite = dto.Civility,
            BirthDate = dto.Birthdate,
            Email = email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IdAspNetUser = userId, // <-- on enregistre l'ID AspNetUser
            PhoneNumber = dto.Phone,
            Pseudo = null,
            Actif = true
        };

        var resp = await api.PostAsJsonAsync("customer", customer, ct);
        if (!resp.IsSuccessStatusCode)
        {
            // à toi de voir : log + continuer ou rollback côté Identity.
            var body = await resp.Content.ReadAsStringAsync(ct);
            return StatusCode((int)resp.StatusCode, new { message = "API customer failed", body });
        }


        return Created($"/api/auth/users/{userId}", new { userId });
    }


    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UpdateUserDto dto, CancellationToken ct)
    {
        var caller = User;

        //// Autorisation: admin OU soi-même
        //var isAdmin = caller.IsInRole("Admin");
        //if (!isAdmin && !IsSelf(caller, id))
        //    return Forbid();

        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound(new { error = "User not found" });

        // Changement d'email si fourni
        if (!string.IsNullOrWhiteSpace(dto.Email) && !string.Equals(dto.Email, user.Email, StringComparison.OrdinalIgnoreCase))
        {
            // option simple : SetEmail + SetUserName (si tu utilises Email comme username)
            var byEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (byEmail is not null) return Conflict(new { error = "Email already used" });

            var setEmailRes = await _userManager.SetEmailAsync(user, dto.Email);
            if (!setEmailRes.Succeeded) return BadRequest(new { errors = setEmailRes.Errors.Select(e => e.Description) });

            var setUserNameRes = await _userManager.SetUserNameAsync(user, dto.Email);
            if (!setUserNameRes.Succeeded) return BadRequest(new { errors = setUserNameRes.Errors.Select(e => e.Description) });
        }

        // Claims "métier" (GivenName/Surname) tenus à jour
        var existingClaims = await _userManager.GetClaimsAsync(user);
        var toRemove = existingClaims.Where(c => c.Type is ClaimTypes.GivenName or ClaimTypes.Surname).ToList();
        if (toRemove.Count > 0) await _userManager.RemoveClaimsAsync(user, toRemove);

        await _userManager.AddClaimsAsync(user, new[]
        {
        new Claim(ClaimTypes.GivenName, dto.FirstName),
        new Claim(ClaimTypes.Surname,  dto.LastName)
    });

        // (Optionnel) autres méta données sur user (si tu en stockes)
        // user.PhoneNumber = dto.Phone; // si tu veux utiliser les champs Identity
        var updateRes = await _userManager.UpdateAsync(user);
        if (!updateRes.Succeeded) return BadRequest(new { errors = updateRes.Errors.Select(e => e.Description) });

        // ----- Propagation vers l’API Customer -----
        // On utilise ton endpoint PUT /customer (soft update)
        var token = await GetClientCredentialsTokenAsync(ct);
        if (!string.IsNullOrEmpty(token))
        {
            var api = ApiWithBearer(token);
            var model = new MinshpWebApp.Api.Request.CustomerRequest
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.Phone,
                Civilite = dto.Civility,
                BirthDate = dto.Birthdate,
                Email = dto.Email ?? user.Email!,
                Pseudo = dto.Pseudo,
                Actif = dto.Actif,
                IdAspNetUser = id,
            };

            // Ton PUT /customer attend un CustomerRequest
            var resp = await api.PutAsJsonAsync("customer", model, ct);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("Customer update failed: {Status} {Body}", resp.StatusCode, body);
                // on n'échoue pas forcément la requête auth si l’API métier n’est pas critique
            }
        }

        return NoContent();
    }

    [HttpPut("{id}/password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromRoute] string id, [FromBody] ChangePasswordDto dto)
    {
        var caller = User;
        //var isAdmin = caller.IsInRole("Admin");
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        //if (!isAdmin && !IsSelf(caller, id)) return Forbid();

        IdentityResult res;

        if (!string.IsNullOrWhiteSpace(dto.CurrentPassword))
        {
            // mode "self‑service"
            res = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword!, dto.NewPassword);
        }
        else
        {
            // mode "admin reset"
            // Attention: avec certains stores, RemovePassword peut échouer si aucun mdp n’est défini.
            if ((await _userManager.HasPasswordAsync(user)))
            {
                res = await _userManager.RemovePasswordAsync(user);
                if (!res.Succeeded) return BadRequest(new { errors = res.Errors.Select(e => e.Description) });
            }
            res = await _userManager.AddPasswordAsync(user, dto.NewPassword);
        }

        if (!res.Succeeded) return BadRequest(new { errors = res.Errors.Select(e => e.Description) });
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser([FromRoute] string id, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        var res = await _userManager.DeleteAsync(user);
        if (!res.Succeeded) return BadRequest(new { errors = res.Errors.Select(e => e.Description) });

        return NoContent();
    }



    [HttpDelete("/desactivated/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DsactivateUser([FromRoute] string id, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        // (Optionnel) soft‑delete côté API
        var token = await GetClientCredentialsTokenAsync(ct);
        if (!string.IsNullOrEmpty(token))
        {
            var api = ApiWithBearer(token);

            // Si tu n’as QUE DELETE /customer/{id:int} dans l’API,
            // et pas de recherche par IdAspNetUser, tu peux faire un "soft‑delete" via PUT :
            var disable = new MinshpWebApp.Api.Request.CustomerRequest
            {
                IdAspNetUser = id,
                Actif = false
            };
            var resp = await api.PutAsJsonAsync("customer", disable, ct);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("Customer disable failed before user delete: {Status} {Body}", resp.StatusCode, body);
            }
        }

        var res = await _userManager.DeleteAsync(user);
        if (!res.Succeeded) return BadRequest(new { errors = res.Errors.Select(e => e.Description) });

        return NoContent();
    }





    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var u = await _userManager.FindByEmailAsync(dto.Email);
        if (u is null) return Unauthorized();
        var res = await _signInManager.PasswordSignInAsync(u, dto.Password, isPersistent: true, lockoutOnFailure: false);
        return res.Succeeded ? Ok(new { message = "logged-in" }) : Unauthorized();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }



    // ---------- Helpers ----------
    private async Task<string?> GetClientCredentialsTokenAsync(CancellationToken ct)
    {
        // Appel simple à /connect/token (IdentityServer) avec le client confidentiel
        var idp = _http.CreateClient(); // client ad hoc
        idp.BaseAddress = new Uri("https://localhost:7183/"); // URL d’IdentityServer

        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = "minshp-api-client",
            ["client_secret"] = "super-secret-change-me",
            ["scope"] = "api"
        });

        using var r = await idp.PostAsync("connect/token", form, ct);
        var body = await r.Content.ReadAsStringAsync(ct);
        if (!r.IsSuccessStatusCode)
        {
            _logger.LogError("Token CC failed: {Status} {Body}", r.StatusCode, body);
            return null;
        }

        var json = await r.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct);
        return json?.access_token;
    }


    private static bool IsSelf(ClaimsPrincipal user, string userId) =>
    user.FindFirst(OpenIddictConstants.Claims.Subject)?.Value == userId;

    private HttpClient ApiWithBearer(string token)
    {
        var api = _http.CreateClient("api");
        api.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return api;
    }

    private sealed record TokenResponse(string token_type, string access_token, int expires_in);

}
