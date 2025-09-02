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
    [Route("Invoices")]
    public class InvoiceController : Controller
    {
        IInvoiceViewModelBuilder _InvoiceViewModelBuilder;

        public InvoiceController(IInvoiceViewModelBuilder InvoiceViewModelBuilder)
        {
            _InvoiceViewModelBuilder = InvoiceViewModelBuilder ?? throw new ArgumentNullException(nameof(InvoiceViewModelBuilder), $"Cannot instantiate {GetType().Name}");
        }


        [HttpGet("/invoices")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<InvoiceViewModel>), Description = "list of categories")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetInvoicesAsync()
        {
            var result = await _InvoiceViewModelBuilder.GetInvoicesAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/invoice")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a Invoice")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutInvoiceAsync([FromBody] InvoiceRequest model)
        {
            var result = await _InvoiceViewModelBuilder.UpdateInvoicesAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/invoice")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a Invoice")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostInvoiceAsync([FromBody] InvoiceRequest model)
        {
            var result = await _InvoiceViewModelBuilder.AddInvoicesAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/invoice/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a Invoice")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteInvoiceAsync([FromRoute] int id)
        {
            var result = await _InvoiceViewModelBuilder.DeleteInvoicesAsync(id);
            return Ok(result);
        }
    }
}
