using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.IdentityServer.Authentication;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;
using OidcClaims = OpenIddict.Abstractions.OpenIddictConstants.Claims;
using OidcDestinations = OpenIddict.Abstractions.OpenIddictConstants.Destinations;

namespace MinshpWebApp.IdentityServer.Controller;

public class AuthorizationController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    public AuthorizationController(UserManager<AppUser> um) { _userManager = um; }

    [HttpGet("~/api/auth/authorize")]
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
            nameType: OidcClaims.Name,
            roleType: OidcClaims.Role);

        identity.AddClaim(OidcClaims.Subject, user.Id);
        identity.AddClaim(OidcClaims.Name, user.UserName ?? string.Empty, Destinations.AccessToken);
        identity.AddClaim(OidcClaims.Email, user.Email ?? string.Empty, Destinations.AccessToken);

        var principal = new ClaimsPrincipal(identity);

        // scopes & resources demandés par le client
        principal.SetScopes(request.GetScopes());
        principal.SetResources("api-resource");

        identity.SetDestinations(claim =>
            claim.Type is OidcClaims.Name or OidcClaims.Email
                ? new[] { Destinations.AccessToken, Destinations.IdentityToken }
                : new[] { Destinations.AccessToken });

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpPost("~/api/auth/logout")]
    [AllowAnonymous]
    public IActionResult LogoutEndpoint() => SignOut(IdentityConstants.ApplicationScheme);
}
