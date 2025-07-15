using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace MinshpWebApp.Api.Controllers
{

    [ApiController]
    [Route("Stocks")]
    public class StockController : Controller
    {
        IStockViewModelBuilder _StockViewModelBuilder;

        public StockController(IStockViewModelBuilder StockViewModelBuilder)
        {
            _StockViewModelBuilder = StockViewModelBuilder ?? throw new ArgumentNullException(nameof(StockViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/stocks")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<StockViewModel>), Description = "list of Stocks")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetStocksAsync()
        {
            var result = await _StockViewModelBuilder.GetStocksAsync();
            return Ok(result);
        }



        [HttpPut("/stock")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a Stock")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutStockAsync([FromBody] StockRequest model)
        {
            var result = await _StockViewModelBuilder.UpdateStocksAsync(model);
            return Ok(result);
        }


        [HttpPost("/stock")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a Stock")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostStockAsync([FromBody] StockRequest model)
        {
            var result = await _StockViewModelBuilder.AddStocksAsync(model);
            return Ok(result);
        }


        [HttpDelete("/stock/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a Stock")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteStockAsync([FromRoute] int id)
        {
            var result = await _StockViewModelBuilder.DeleteStocksAsync(id);
            return Ok(result);
        }
    }
}
