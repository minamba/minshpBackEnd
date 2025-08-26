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
    [Route("Orders")]
    public class OrderController : Controller
    {
        IOrderViewModelBuilder _orderViewModelBuilder;

        public OrderController(IOrderViewModelBuilder orderViewModelBuilder)
        {
            _orderViewModelBuilder = orderViewModelBuilder ?? throw new ArgumentNullException(nameof(orderViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/orders")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<OrderViewModel>), Description = "list of categories")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetOrdersAsync()
        {
            var result = await _orderViewModelBuilder.GetOrdersAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/order")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a order")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutOrderAsync([FromBody] OrderRequest model)
        {
            var result = await _orderViewModelBuilder.UpdateOrdersAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/order")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a order")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostOrderAsync([FromBody] OrderRequest model)
        {
            var result = await _orderViewModelBuilder.AddOrdersAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/order/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a order")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteOrderAsync([FromRoute] int id)
        {
            var result = await _orderViewModelBuilder.DeleteOrdersAsync(id);
            return Ok(result);
        }
    }
}
