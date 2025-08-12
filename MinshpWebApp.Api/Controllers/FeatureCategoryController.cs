using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace MinshpWebApp.Api.Controllers
{
    [ApiController]
    [Route("FeatureCategory")]
    public class FeatureCategoryController : Controller
    {
        IFeatureCategoryViewModelBuilder _featureCategoryViewModelBuilder;

        public FeatureCategoryController(IFeatureCategoryViewModelBuilder featureCategoryViewModelBuilder)
        {
            _featureCategoryViewModelBuilder = featureCategoryViewModelBuilder ?? throw new ArgumentNullException(nameof(featureCategoryViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/featureCategories")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FeatureCategoryViewModel>), Description = "list of FeatureCategorys")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetFeatureCategorysAsync()
        {
            var result = await _featureCategoryViewModelBuilder.GetFeatureCategoriesAsync();
            return Ok(result);
        }



        [HttpGet("/featuresCategoryByProduct/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FeatureCategoryViewModel>), Description = "list of Feature category by product for the specs display")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetFeaturesCategoryByProductAsync([FromRoute] int id)
        {
            var result = await _featureCategoryViewModelBuilder.GetFeaturesCategoryProductAsync(id);
            return Ok(result);
        }




        [HttpPut("/featureCategory")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a FeatureCategory")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutFeatureCategoryAsync([FromBody] FeatureCategoryRequest model)
        {
            var result = await _featureCategoryViewModelBuilder.UpdateFeatureCategoryAsync(model);
            return Ok(result);
        }


        [HttpPost("/featureCategory")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a FeatureCategory")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostFeatureCategoryAsync([FromBody] FeatureCategoryRequest model)
        {
            var result = await _featureCategoryViewModelBuilder.AddFeatureCategoryAsync(model);
            return Ok(result);
        }


        [HttpDelete("/featureCategory/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a FeatureCategory")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteFeatureCategoryAsync([FromRoute] int id)
        {
            var result = await _featureCategoryViewModelBuilder.DeleteFeatureCategoryAsync(id);
            return Ok(result);
        }
    }
}
