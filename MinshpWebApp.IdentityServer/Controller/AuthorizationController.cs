using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.IdentityServer.Authentication;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace MinshpWebApp.IdentityServer.Controllers;

public class AuthorizationController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    public AuthorizationController(UserManager<AppUser> um) { _userManager = um; }

    [HttpGet("~/connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
                      ?? throw new InvalidOperationException("OIDC request is null.");

        // L’utilisateur doit être connecté (cookie Identity) pour émettre un code
        if (!User.Identity?.IsAuthenticated ?? true)
            return Challenge(IdentityConstants.ApplicationScheme);

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Forbid();

        var identity = new ClaimsIdentity(
            authenticationType: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.AddClaim(Claims.Subject, user.Id);
        identity.AddClaim(Claims.Name, user.UserName ?? string.Empty, Destinations.AccessToken);
        identity.AddClaim(Claims.Email, user.Email ?? string.Empty, Destinations.AccessToken);

        var principal = new ClaimsPrincipal(identity);

        // scopes & resources demandés par le client
        principal.SetScopes(request.GetScopes());
        principal.SetResources("api-resource");

        identity.SetDestinations(claim =>
            claim.Type is Claims.Name or Claims.Email
                ? new[] { Destinations.AccessToken, Destinations.IdentityToken }
                : new[] { Destinations.AccessToken });

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpPost("~/connect/logout")]
    [AllowAnonymous]
    public IActionResult LogoutEndpoint() => SignOut(IdentityConstants.ApplicationScheme);
}
