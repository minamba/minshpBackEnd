using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace MinshpWebApp.Api.Controllers
{
    [ApiController]
    [Route("Products")]
    public class ProductController : Controller
    {
        IProductViewModelBuilder _productViewModelBuilder;


        public ProductController(IProductViewModelBuilder productViewModelBuilder)
        {
            _productViewModelBuilder = productViewModelBuilder ?? throw new ArgumentNullException(nameof(productViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/products")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ProductVIewModel>), Description = "list of products")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetProductsAsync()
        {
            var result = await _productViewModelBuilder.GetProductsAsync();
            return Ok(result);
        }



        [HttpPut("/product")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a product")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutProductAsync([FromBody] ProductRequest model)
        {
            var result = await _productViewModelBuilder.UpdateProductsAsync(model);
            return Ok(result);
        }


        [HttpPost("/product")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a product")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostProductAsync([FromBody] ProductRequest model)
        {
            var result = await _productViewModelBuilder.AddProductsAsync(model);
            return Ok(result);
        }


        [HttpDelete("/product/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a product")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteProductAsync([FromRoute] int id)
        {
            var result = await _productViewModelBuilder.DeleteProductsAsync(id);
            return Ok(result);
        }
    }
}
