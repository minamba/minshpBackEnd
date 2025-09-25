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
    [Route("newsLetter")]
    public class NewLetterController : Controller
    {
        INewLetterViewModelBuilder _NewLetterViewModelBuilder;

        public NewLetterController(INewLetterViewModelBuilder NewLetterViewModelBuilder)
        {
            _NewLetterViewModelBuilder = NewLetterViewModelBuilder ?? throw new ArgumentNullException(nameof(NewLetterViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/newLetters")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<NewLetterViewModel>), Description = "list of NewLetters")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetNewLettersAsync()
        {
            var result = await _NewLetterViewModelBuilder.GetNewLetterssAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/newLetter")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a NewLetter")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutNewLetterAsync([FromBody] NewLetterRequest model)
        {
            var result = await _NewLetterViewModelBuilder.UpdateNewLettersAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/newLetter")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a NewLetter")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostNewLetterAsync([FromBody] NewLetterRequest model)
        {
            var result = await _NewLetterViewModelBuilder.AddNewLettersAsync(model);

            if(result.find == false)
              return Ok(result.message);
            else
              return BadRequest(result.message);
        }

        [Authorize]
        [HttpDelete("/newLetter/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a NewLetter")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteNewLetterAsync([FromRoute] int id)
        {
            var result = await _NewLetterViewModelBuilder.DeleteNewLettersAsync(id);
            return Ok(result);
        }
    }
}
