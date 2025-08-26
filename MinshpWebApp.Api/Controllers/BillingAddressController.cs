using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace MinshpWebApp.Api.Controllers
{
    public class BillingAddressController : Controller
    {
        IBillingAddressViewModelBuilder _billingAddressViewModelBuilder;

        public BillingAddressController(IBillingAddressViewModelBuilder billingAddressViewModelBuilder)
        {
            _billingAddressViewModelBuilder = billingAddressViewModelBuilder ?? throw new ArgumentNullException(nameof(billingAddressViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/billingAddresses")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<BillingAddressViewModel>), Description = "list of categories")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetBillingAddressesAsync()
        {
            var result = await _billingAddressViewModelBuilder.GetBillingAddressesAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/billingAddress")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a billingAddress")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutBillingAddressAsync([FromBody] BillingAddressRequest model)
        {
            var result = await _billingAddressViewModelBuilder.UpdateBillingAddressAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/billingAddress")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a billingAddress")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostBillingAddressAsync([FromBody] BillingAddressRequest model)
        {
            var result = await _billingAddressViewModelBuilder.AddBillingAddresssAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/billingAddress/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a billingAddress")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteBillingAddressAsync([FromRoute] int id)
        {
            var result = await _billingAddressViewModelBuilder.DeleteBillingAddresssAsync(id);
            return Ok(result);
        }
    }
}
