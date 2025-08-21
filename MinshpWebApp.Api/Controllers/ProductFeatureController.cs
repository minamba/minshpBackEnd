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
    [Route("ProductFeatures")]
    public class ProductFeatureController : Controller
    {
        IProductFeatureViewModelBuilder _ProductFeatureViewModelBuilder;

        public ProductFeatureController(IProductFeatureViewModelBuilder ProductFeatureViewModelBuilder)
        {
            _ProductFeatureViewModelBuilder = ProductFeatureViewModelBuilder ?? throw new ArgumentNullException(nameof(ProductFeatureViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/productFeatures")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ProductFeatureViewModel>), Description = "list of ProductFeatures")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetProductFeaturesAsync()
        {
            var result = await _ProductFeatureViewModelBuilder.GetProductFeaturesAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/productFeature")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a ProductFeature")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutProductFeatureAsync([FromBody] ProductFeatureRequest model)
        {
            var result = await _ProductFeatureViewModelBuilder.UpdateProductFeaturesAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/productFeature")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a ProductFeature")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostProductFeatureAsync([FromBody] ProductFeatureRequest model)
        {
            var result = await _ProductFeatureViewModelBuilder.AddProductFeaturesAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/productFeature/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a ProductFeature")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteProductFeatureAsync([FromRoute] int id)
        {
            var result = await _ProductFeatureViewModelBuilder.DeleteProductFeaturesAsync(id);
            return Ok(result);
        }
    }
}
