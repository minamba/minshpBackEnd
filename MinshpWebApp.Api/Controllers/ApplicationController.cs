using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace MinshpWebApp.Api.Controllers
{
    [ApiController]
    [Route("Applications")]
    public class ApplicationController : Controller
    {
        IApplicationViewModelBuilder _applicationViewModelBuilder;

        public ApplicationController(IApplicationViewModelBuilder applicationViewModelBuilder)
        {
            _applicationViewModelBuilder = applicationViewModelBuilder ?? throw new ArgumentNullException(nameof(applicationViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/application")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ApplicationViewModel>), Description = "list of applications")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetApplicationsAsync()
        {
            var result = await _applicationViewModelBuilder.GetApplicationAsync();
            return Ok(result);
        }



        [HttpPut("/application")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a application")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutApplicationAsync([FromBody] ApplicationRequest model)
        {
            var result = await _applicationViewModelBuilder.UpdateApplicationsAsync(model);
            return Ok(result);
        }


        [HttpPost("/application")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a application")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostApplicationAsync([FromBody] ApplicationRequest model)
        {
            var result = await _applicationViewModelBuilder.AddApplicationsAsync(model);
            return Ok(result);
        }


        [HttpDelete("/application/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a application")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteApplicationtAsync([FromRoute] int id)
        {
            var result = await _applicationViewModelBuilder.DeleteApplicationsAsync(id);
            return Ok(result);
        }
    }
}
