using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace MinshpWebApp.Api.Controllers
{

    [ApiController]
    [Route("roles")]
    public class RoleController : Controller
    {
        IRoleViewModelBuilder _roleViewModelBuilder;

        public RoleController(IRoleViewModelBuilder roleViewModelBuilder)
        {
            _roleViewModelBuilder = roleViewModelBuilder ?? throw new ArgumentNullException(nameof(roleViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/roles")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<AspNetRoleViewModel>), Description = "list of roles")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetRolesAsync()
        {
            var result = await _roleViewModelBuilder.GetRolesAsync();
            return Ok(result);
        }
    }
}
