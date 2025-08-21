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

namespace MinshpWebApp.IdentityServer.Controllers;

[ApiController]
public class TokenController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public TokenController(UserManager<AppUser> um, SignInManager<AppUser> sm)
    {
        _userManager = um; _signInManager = sm;
    }

    [HttpPost("~/connect/token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("OpenIddict request is null.");

        if (request.IsPasswordGrantType())
        {
            var user = await _userManager.FindByEmailAsync(request.Username ?? "");
            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password ?? ""))
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var identity = new ClaimsIdentity(
                authenticationType: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                nameType: Claims.Name, roleType: Claims.Role);

            identity.AddClaim(Claims.Subject, user.Id);
            identity.AddClaim(Claims.Name, user.UserName ?? "");
            identity.AddClaim(Claims.Email, user.Email ?? "");

            // où doivent aller les claims
            identity.SetDestinations(claim =>
                claim.Type is Claims.Name or Claims.Email
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

        throw new InvalidOperationException("Unsupported grant type.");
    }
}
