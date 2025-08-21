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
    [Route("Taxes")]
    public class TaxeController : Controller
    {
        ITaxeViewModelBuilder _taxeViewModelBuilder;

        public TaxeController(ITaxeViewModelBuilder TaxeViewModelBuilder)
        {
            _taxeViewModelBuilder = TaxeViewModelBuilder ?? throw new ArgumentNullException(nameof(TaxeViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/taxes")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<TaxeViewModel>), Description = "list of Taxes")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetTaxesAsync()
        {
            var result = await _taxeViewModelBuilder.GetTaxesAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/taxe")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a Taxe")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutTaxeAsync([FromBody] TaxeRequest model)
        {
            var result = await _taxeViewModelBuilder.UpdateTaxeAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/taxe")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a Taxe")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostTaxeAsync([FromBody] TaxeRequest model)
        {
            var result = await _taxeViewModelBuilder.AddTaxeAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/taxe/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a Taxe")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteTaxeAsync([FromRoute] int id)
        {
            var result = await _taxeViewModelBuilder.DeleteTaxeAsync(id);
            return Ok(result);
        }
    }
}
