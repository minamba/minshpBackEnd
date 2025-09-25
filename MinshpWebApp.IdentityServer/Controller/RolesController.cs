using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.IdentityServer.Authentication;

namespace MinshpWebApp.IdentityServer.Controller
{
    [ApiController]
    [Route("role")]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<AppUser> _users;
        public RolesController(UserManager<AppUser> users) => _users = users;

        public record ChangeRoleDto(string UserId, string Role);

        [HttpPost("add")]
        public async Task<IActionResult> AddRole(ChangeRoleDto dto)
        {
            var user = await _users.FindByIdAsync(dto.UserId);
            if (user is null) return NotFound("User not found");

            var res = await _users.AddToRoleAsync(user, dto.Role);
            return res.Succeeded ? Ok() : BadRequest(res.Errors);
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveRole(ChangeRoleDto dto)
        {
            var user = await _users.FindByIdAsync(dto.UserId);
            if (user is null) return NotFound("User not found");

            var res = await _users.RemoveFromRoleAsync(user, dto.Role);
            return res.Succeeded ? Ok() : BadRequest(res.Errors);
        }
    }
}
