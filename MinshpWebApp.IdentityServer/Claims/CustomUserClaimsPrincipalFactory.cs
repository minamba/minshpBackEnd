using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MinshpWebApp.IdentityServer.Authentication;
using System.Security.Claims;

namespace MinshpWebApp.IdentityServer.Claims
{

    public class CustomUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<AppUser, IdentityRole>
    {
        private readonly UserManager<AppUser> _userManager;
        public CustomUserClaimsPrincipalFactory(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
            _userManager = userManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            // Récupère les rôles Identity et les ajoute en "role"
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
                identity.AddClaim(new Claim(ClaimTypes.Role, role)); // => "role" dans le token

            return identity;
        }
    }
}
