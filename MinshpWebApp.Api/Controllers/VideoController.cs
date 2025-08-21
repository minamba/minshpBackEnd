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
    [Route("Videos")]
    public class VideoController : Controller
    {
        IVideoViewModelBuilder _videoViewModelBuilder;

        public VideoController(IVideoViewModelBuilder VideoViewModelBuilder)
        {
            _videoViewModelBuilder = VideoViewModelBuilder ?? throw new ArgumentNullException(nameof(VideoViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/videos")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<VideoViewModel>), Description = "list of Videos")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetVideosAsync()
        {
            var result = await _videoViewModelBuilder.GetVideosAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/video")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a Video")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutVideoAsync([FromBody] VideoRequest model)
        {
            var result = await _videoViewModelBuilder.UpdateVideoAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/video")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a Video")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostVideoAsync([FromBody] VideoRequest model)
        {
            var result = await _videoViewModelBuilder.AddVideoAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/video/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a Video")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteVideoAsync([FromRoute] int id)
        {
            var result = await _videoViewModelBuilder.DeleteVideoAsync(id);
            return Ok(result);
        }
    }
}
