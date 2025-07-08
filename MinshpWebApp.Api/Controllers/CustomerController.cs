using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace MinshpWebApp.Api.Controllers
{
    [ApiController]
    [Route("Customers")]
    public class CustomerController : Controller
    {
        ICustomerViewModelBuilder _customerViewModelBuilder;

        public CustomerController(ICustomerViewModelBuilder customerViewModelBuilder)
        {
            _customerViewModelBuilder = customerViewModelBuilder ?? throw new ArgumentNullException(nameof(customerViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/customers")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<CustomerViewModel>), Description = "list of Customers")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetCustomerssAsync()
        {
            var result = await _customerViewModelBuilder.GetCustomersAsync();
            return Ok(result);
        }



        [HttpPut("/customer")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a Customer")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutCustomerAsync([FromBody] CustomerRequest model)
        {
            var result = await _customerViewModelBuilder.UpdateCustomersAsync(model);
            return Ok(result);
        }


        [HttpPost("/customer")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a Customer")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostCustomerAsync([FromBody] CustomerRequest model)
        {
            var result = await _customerViewModelBuilder.AddCustomersAsync(model);
            return Ok(result);
        }


        [HttpDelete("/customer/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a Customer")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteCustomerAsync([FromRoute] int id)
        {
            var result = await _customerViewModelBuilder.DeleteCustomersAsync(id);
            return Ok(result);
        }
    }
}
