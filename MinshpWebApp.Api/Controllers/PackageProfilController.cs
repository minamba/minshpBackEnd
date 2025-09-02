using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace MinshpWebApp.Api.Controllers
{
    [ApiController]
    [Route("PackageProfils")]
    public class PackageProfilController : ControllerBase
    {
        IPackageProfilViewModelBuilder _PackageProfilViewModelBuilder;

        public PackageProfilController(IPackageProfilViewModelBuilder PackageProfilViewModelBuilder)
        {
            _PackageProfilViewModelBuilder = PackageProfilViewModelBuilder ?? throw new ArgumentNullException(nameof(PackageProfilViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/packageProfils")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<PackageProfilViewModel>), Description = "list of PackageProfils")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetPackageProfilsAsync()
        {
            var result = await _PackageProfilViewModelBuilder.GetPackageProfilsAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/packageProfil")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a PackageProfil")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutPackageProfilAsync([FromBody] PackageProfilRequest model)
        {
            var result = await _PackageProfilViewModelBuilder.UpdatePackageProfilsAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/packageProfil")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a PackageProfil")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostPackageProfilAsync([FromBody] PackageProfilRequest model)
        {
            var result = await _PackageProfilViewModelBuilder.AddPackageProfilsAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/packageProfil/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a PackageProfil")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeletePackageProfilAsync([FromRoute] int id)
        {
            var result = await _PackageProfilViewModelBuilder.DeletePackageProfilsAsync(id);
            return Ok(result);
        }
    }
}
