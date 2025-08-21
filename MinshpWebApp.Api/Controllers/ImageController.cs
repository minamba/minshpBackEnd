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
    [Route("Images")]
    public class ImageController : Controller
    {
        IImageViewModelBuilder _imageViewModelBuilder;

        public ImageController(IImageViewModelBuilder ImageViewModelBuilder)
        {
            _imageViewModelBuilder = ImageViewModelBuilder ?? throw new ArgumentNullException(nameof(ImageViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/images")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ImageViewModel>), Description = "list of Images")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetImagesAsync()
        {
            var result = await _imageViewModelBuilder.GetImagesAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/image")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a Image")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutImageAsync([FromBody] ImageRequest model)
        {
            var result = await _imageViewModelBuilder.UpdateImagesAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/image")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a Image")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostImageAsync([FromBody] ImageRequest model)
        {
            var result = await _imageViewModelBuilder.AddImagesAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/image/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a Image")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteImageAsync([FromRoute] int id)
        {
            var result = await _imageViewModelBuilder.DeleteImagesAsync(id);
            return Ok(result);
        }
    }
}
