using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.ViewModels;
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
    }
}
