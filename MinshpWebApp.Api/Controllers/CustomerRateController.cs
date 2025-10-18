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
    [Route("CustomerRates")]
    public class CustomerRateController : Controller
    {
        ICustomerRateViewModelBuilder _customerRateViewModelBuilder;

        public CustomerRateController(ICustomerRateViewModelBuilder CustomerRateViewModelBuilder)
        {
            _customerRateViewModelBuilder = CustomerRateViewModelBuilder ?? throw new ArgumentNullException(nameof(CustomerRateViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/customerRates")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<CustomerRateViewModel>), Description = "list of CustomerRates")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetCustomerRatesAsync()
        {
            var result = await _customerRateViewModelBuilder.GetCustomerRatesAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/customerRate")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a CustomerRate")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutCustomerRateAsync([FromBody] CustomerRateRequest model)
        {
            var result = await _customerRateViewModelBuilder.UpdateCustomerRateAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/customerRate")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a CustomerRate")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostCustomerRateAsync([FromBody] CustomerRateRequest model)
        {
            var result = await _customerRateViewModelBuilder.AddCustomerRateAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/customerRate/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a CustomerRate")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteCustomerRateAsync([FromRoute] int id)
        {
            var result = await _customerRateViewModelBuilder.DeleteCustomerRateAsync(id);
            return Ok(result);
        }
    }
}
