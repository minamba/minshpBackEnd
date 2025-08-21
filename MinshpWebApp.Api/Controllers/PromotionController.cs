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
    [Route("Promotions")]
    public class PromotionController : Controller
    {
        IPromotionViewModelBuilder _promotionViewModelBuilder;

        public PromotionController(IPromotionViewModelBuilder promotionViewModelBuilder)
        {
            _promotionViewModelBuilder = promotionViewModelBuilder ?? throw new ArgumentNullException(nameof(promotionViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/promotions")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<PromotionViewModel>), Description = "list of promotions")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetPromotionsAsync()
        {
            var result = await _promotionViewModelBuilder.GetPromotionsAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/promotion")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a promotion")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutPromotionAsync([FromBody] PromotionRequest model)
        {
            var result = await _promotionViewModelBuilder.UpdatePromotionsAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/promotion")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a promotion")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostPromotionAsync([FromBody] PromotionRequest model)
        {
            var result = await _promotionViewModelBuilder.AddPromotionsAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/promotion/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a promotion")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeletePromotiontAsync([FromRoute] int id)
        {
            var result = await _promotionViewModelBuilder.DeletePromotionsAsync(id);
            return Ok(result);
        }
    }
}
