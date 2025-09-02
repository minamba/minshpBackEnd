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
    [Route("OrderCustomerProducts")]
    public class OrderCustomerProductController : Controller
    {
        IOrderCustomerProductViewModelBuilder _OrderCustomerProductViewModelBuilder;


        public OrderCustomerProductController(IOrderCustomerProductViewModelBuilder OrderCustomerProductViewModelBuilder)
        {
            _OrderCustomerProductViewModelBuilder = OrderCustomerProductViewModelBuilder ?? throw new ArgumentNullException(nameof(OrderCustomerProductViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/orderCustomerProducts")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<OrderCustomerProductViewModel>), Description = "list of OrderCustomerProducts")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetOrderCustomerProductsAsync()
        {
            var result = await _OrderCustomerProductViewModelBuilder.GetOrderCustomerProductsAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/orderCustomerProduct")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a OrderCustomerProduct")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutOrderCustomerProductAsync([FromBody] OrderCustomerProductRequest model)
        {
            var result = await _OrderCustomerProductViewModelBuilder.UpdateOrderCustomerProductsAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/orderCustomerProduct")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a OrderCustomerProduct")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostOrderCustomerProductAsync([FromBody] OrderCustomerProductRequest model)
        {
            var result = await _OrderCustomerProductViewModelBuilder.AddOrderCustomerProductsAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/orderCustomerProduct")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a OrderCustomerProduct")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteOrderCustomerProductAsync([FromBody] OrderCustomerProductRequest model)
        {
            var result = await _OrderCustomerProductViewModelBuilder.DeleteOrderCustomerProductsAsync(model);
            return Ok(result);
        }
    }
}
