// Controllers/TokenController.cs
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.IdentityServer.Authentication;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;
using OidcClaims = OpenIddict.Abstractions.OpenIddictConstants.Claims;
using OidcDestinations = OpenIddict.Abstractions.OpenIddictConstants.Destinations;

namespace MinshpWebApp.IdentityServer.Controllers;

[ApiController]
public class TokenController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public TokenController(UserManager<AppUser> um, SignInManager<AppUser> sm)
    {
        _userManager = um; _signInManager = sm;
    }

    [HttpPost("~/api/auth/token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("OpenIddict request is null.");

        if (request.IsPasswordGrantType())
        {
            var user = await _userManager.FindByEmailAsync(request.Username ?? "");
            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password ?? ""))
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Déjà bloqué ?
            if (await _userManager.IsLockedOutAsync(user))
                return InvalidGrant("ACCOUNT_LOCKED: Votre compte est temporairement bloqué.");

            // Vérifie le mot de passe et incrémente les échecs (lockoutOnFailure: true)
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password ?? "", lockoutOnFailure: true);

            if (result.IsLockedOut)
                return InvalidGrant("ACCOUNT_LOCKED: Votre compte est temporairement bloqué suite à des tentatives infructueuses.");

            if (!result.Succeeded)
                return InvalidGrant("INVALID_CREDENTIALS");


            var identity = new ClaimsIdentity(
                authenticationType: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                nameType: OidcClaims.Name, roleType: OidcClaims.Role);

            identity.AddClaim(OidcClaims.Subject, user.Id);
            identity.AddClaim(OidcClaims.Name, user.UserName ?? "");
            identity.AddClaim(OidcClaims.Email, user.Email ?? "");


            // ✅ Ajouter les rôles
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var r in roles)
                identity.AddClaim(OidcClaims.Role, r);


            // où doivent aller les claims
            identity.SetDestinations(claim =>
                claim.Type is OidcClaims.Name or OidcClaims.Email or OidcClaims.Role
                    ? new[] { Destinations.AccessToken, Destinations.IdentityToken }
                    : new[] { Destinations.AccessToken });

            var principal = new ClaimsPrincipal(identity);
            principal.SetScopes(request.GetScopes());
            principal.SetResources("api-resource");

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsRefreshTokenGrantType())
        {
            // OpenIddict récupère le principal du RT automatiquement
            var authenticateResult = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            var principal = authenticateResult.Principal!;
            principal.SetScopes(request.GetScopes());
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        if (request.IsClientCredentialsGrantType())
        {
            // Pas d’utilisateur ici : on émet un access_token “application”
            var identity = new ClaimsIdentity(
                authenticationType: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                nameType: OidcClaims.Name,
                roleType: OidcClaims.Role);

            // Optionnel : identifie le client dans le token
            identity.AddClaim(OidcClaims.Subject, request.ClientId ?? string.Empty);
            identity.AddClaim(OidcClaims.ClientId, request.ClientId ?? string.Empty);

            // Destinations (access_token uniquement)
            identity.SetDestinations(_ => new[] { Destinations.AccessToken });

            var principal = new ClaimsPrincipal(identity);

            // Reprend les scopes demandés (ex: "api")
            principal.SetScopes(request.GetScopes());

            // Déclare la ressource/API si tu l’utilises
            principal.SetResources("api-resource");

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }


        throw new InvalidOperationException("Unsupported grant type.");
    }

    private ForbidResult InvalidGrant(string description)
    {
        var props = new AuthenticationProperties(new Dictionary<string, string?>
        {
            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = description
        });

        return Forbid(props, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
