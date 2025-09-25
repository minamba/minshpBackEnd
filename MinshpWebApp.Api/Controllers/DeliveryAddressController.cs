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
    [Route("DeliveryAddresses")]
    public class DeliveryAddressController : Controller
    {
        IDeliveryAddressViewModelBuilder _deliveryAddressViewModelBuilder;

        public DeliveryAddressController(IDeliveryAddressViewModelBuilder deliveryAddressViewModelBuilder)
        {
            _deliveryAddressViewModelBuilder = deliveryAddressViewModelBuilder ?? throw new ArgumentNullException(nameof(deliveryAddressViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/deliveryAddresses")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<DeliveryAddressViewModel>), Description = "list of categories")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetDeliveryAddressesAsync()
        {
            var result = await _deliveryAddressViewModelBuilder.GetDeliveryAddressesAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/deliveryAddress")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a deliveryAddress")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutDeliveryAddressAsync([FromBody] DeliveryAddressRequest model)
        {
            var result = await _deliveryAddressViewModelBuilder.UpdateDeliveryAddressAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/deliveryAddress")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a deliveryAddress")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostDeliveryAddressAsync([FromBody] DeliveryAddressRequest model)
        {
            var result = await _deliveryAddressViewModelBuilder.AddDeliveryAddresssAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/deliveryAddress/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a deliveryAddress")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteDeliveryAddressAsync([FromRoute] int id)
        {
            var result = await _deliveryAddressViewModelBuilder.DeleteDeliveryAddresssAsync(id);
            return Ok(result);
        }
    }
}
