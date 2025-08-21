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
    [Route("Categories")]
    public class CategoryController : Controller
    {
        ICategoryViewModelBuilder _categoryViewModelBuilder;

        public CategoryController(ICategoryViewModelBuilder categoryViewModelBuilder)
        {
            _categoryViewModelBuilder = categoryViewModelBuilder ?? throw new ArgumentNullException(nameof(categoryViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/categories")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<CategoryViewModel>), Description = "list of categories")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            var result = await _categoryViewModelBuilder.GetCategoriesAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/category")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a category")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutCategoryAsync([FromBody] CategoryRequest model)
        {
            var result = await _categoryViewModelBuilder.UpdateCategorysAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/category")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a category")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostCategoryAsync([FromBody] CategoryRequest model)
        {
            var result = await _categoryViewModelBuilder.AddCategorysAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/category/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a category")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id)
        {
            var result = await _categoryViewModelBuilder.DeleteCategorysAsync(id);
            return Ok(result);
        }
    }
}
