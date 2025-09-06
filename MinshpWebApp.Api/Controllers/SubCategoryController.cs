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
    [Route("SubCategories")]
    public class SubCategoryController : ControllerBase
    {
        ISubCategoryViewModelBuilder _SubCategoryViewModelBuilder;

        public SubCategoryController(ISubCategoryViewModelBuilder SubCategoryViewModelBuilder)
        {
            _SubCategoryViewModelBuilder = SubCategoryViewModelBuilder ?? throw new ArgumentNullException(nameof(SubCategoryViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/subCategories")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<SubCategoryViewModel>), Description = "list of SubCategorys")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetSubCategorysAsync()
        {
            var result = await _SubCategoryViewModelBuilder.GetSubCategoriesAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/subCategory")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a SubCategory")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutSubCategoryAsync([FromBody] SubCategoryRequest model)
        {
            var result = await _SubCategoryViewModelBuilder.UpdateSubCategorysAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/subCategory")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a SubCategory")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostSubCategoryAsync([FromBody] SubCategoryRequest model)
        {
            var result = await _SubCategoryViewModelBuilder.AddSubCategorysAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/subCategory/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a SubCategory")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteSubCategoryAsync([FromRoute] int id)
        {
            var result = await _SubCategoryViewModelBuilder.DeleteSubCategorysAsync(id);
            return Ok(result);
        }
    }
}
