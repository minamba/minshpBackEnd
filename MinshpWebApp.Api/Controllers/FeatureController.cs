using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace MinshpWebApp.Api.Controllers
{
    [ApiController]
    [Route("features")]
    public class FeatureController : Controller
    {
        IFeatureViewModelBuilder _featureViewModelBuilder;

        public FeatureController(IFeatureViewModelBuilder FeatureViewModelBuilder)
        {
            _featureViewModelBuilder = FeatureViewModelBuilder ?? throw new ArgumentNullException(nameof(FeatureViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/feature")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FeatureViewModel>), Description = "list of features")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetFeaturesAsync()
        {
            var result = await _featureViewModelBuilder.GetFeaturesAsync();
            return Ok(result);
        }



        [HttpPut("/feature")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a Feature")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutFeatureAsync([FromBody] FeatureRequest model)
        {
            var result = await _featureViewModelBuilder.UpdateFeaturesAsync(model);
            return Ok(result);
        }


        [HttpPost("/feature")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a Feature")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostFeatureAsync([FromBody] FeatureRequest model)
        {
            var result = await _featureViewModelBuilder.AddFeaturesAsync(model);
            return Ok(result);
        }


        [HttpDelete("/feature/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a Feature")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteFeatureAsync([FromRoute] int id)
        {
            var result = await _featureViewModelBuilder.DeleteFeaturesAsync(id);
            return Ok(result);
        }
    }
}
