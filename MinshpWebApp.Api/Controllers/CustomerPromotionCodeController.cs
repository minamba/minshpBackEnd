using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Builders.impl;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace MinshpWebApp.Api.Controllers
{
    [ApiController]
    [Route("CustomerPromotions")]
    public class CustomerPromotionCodeController : Controller
    {
        ICustomerPromotionCodeViewModelBuilder _customerPromotionCodeViewModelBuilder;

        public CustomerPromotionCodeController(ICustomerPromotionCodeViewModelBuilder customerPromotionCodeViewModelBuilder)
        {
            _customerPromotionCodeViewModelBuilder = customerPromotionCodeViewModelBuilder ?? throw new ArgumentNullException(nameof(CustomerPromotionCodeViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/customerPromotionCodes")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<CustomerPromotionCodeViewModel>), Description = "list of CustomerPromotionCodes")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetCustomerPromotionCodesAsync()
        {
            var result = await _customerPromotionCodeViewModelBuilder.GetCustomerPromotionCodesAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/customerPromotionCode")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a CustomerPromotionCode")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutCustomerPromotionCodeAsync([FromBody] CustomerPromotionCodeRequest model)
        {
            var result = await _customerPromotionCodeViewModelBuilder.UpdateCustomerPromotionCodesAsync(model);
            return Ok(result);
        }

        //[Authorize]
        [HttpPost("/customerPromotionCode")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a CustomerPromotionCode")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostCustomerPromotionCodeAsync([FromBody] CustomerPromotionCodeRequest model)
        {
            var result = await _customerPromotionCodeViewModelBuilder.AddCustomerPromotionCodesAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/customerPromotionCode/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a CustomerPromotionCode")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteCustomerPromotionCodeAsync([FromRoute] int id)
        {
            var result = await _customerPromotionCodeViewModelBuilder.DeleteCustomerPromotionCodesAsync(id);
            return Ok(result);
        }
    }
}
