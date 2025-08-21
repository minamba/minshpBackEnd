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
    [Route("PromotionCodes")]
    public class PromotionCodeController : Controller
    {
        IPromotionCodeViewModelBuilder _promotionCodeViewModelBuilder;

        public PromotionCodeController(IPromotionCodeViewModelBuilder PromotionCodeViewModelBuilder)
        {
            _promotionCodeViewModelBuilder = PromotionCodeViewModelBuilder ?? throw new ArgumentNullException(nameof(PromotionCodeViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/promotionCodes")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<PromotionCodeViewModel>), Description = "list of PromotionCodes")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetPromotionCodesAsync()
        {
            var result = await _promotionCodeViewModelBuilder.GetPromotionCodesAsync();
            return Ok(result);
        }

        [Authorize]

        [HttpPut("/promotionCode")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a PromotionCode")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutPromotionCodeAsync([FromBody] PromotionCodeRequest model)
        {
            var result = await _promotionCodeViewModelBuilder.UpdatePromotionCodesAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/promotionCode")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a PromotionCode")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostPromotionCodeAsync([FromBody] PromotionCodeRequest model)
        {
            var result = await _promotionCodeViewModelBuilder.AddPromotionCodesAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/promotionCode/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a PromotionCode")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeletePromotionCodeAsync([FromRoute] int id)
        {
            var result = await _promotionCodeViewModelBuilder.DeletePromotionCodesAsync(id);
            return Ok(result);
        }
    }
}
