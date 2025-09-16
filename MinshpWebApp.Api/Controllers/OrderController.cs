using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Builders.impl;
using MinshpWebApp.Api.Options;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using StripeRequest = MinshpWebApp.Api.Request.StripeRequest;

namespace MinshpWebApp.Api.Controllers
{
    [ApiController]
    [Route("Orders")]
    public class OrderController : Controller
    {
        private readonly IOrderViewModelBuilder _orderViewModelBuilder;
        private readonly IConfiguration _configuration;
        private readonly StripeSettings _stripe;
        private readonly string _frontendBaseUrl;
        private readonly IInvoiceViewModelBuilder _invoiceViewModelBuilder;


        public OrderController(IOrderViewModelBuilder orderViewModelBuilder, IOptions<StripeSettings> stripe, IConfiguration configuration, IInvoiceViewModelBuilder invoiceViewModelBuilder )
        {
            _orderViewModelBuilder = orderViewModelBuilder ?? throw new ArgumentNullException(nameof(orderViewModelBuilder), $"Cannot instantiate {GetType().Name}");
            _stripe = stripe.Value ?? throw new ArgumentNullException(nameof(stripe), $"Cannot instantiate {GetType().Name}");
            _frontendBaseUrl = configuration["FrontendBaseUrl"];
            _invoiceViewModelBuilder = invoiceViewModelBuilder;
        }


        [HttpGet("/orders")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<OrderViewModel>), Description = "list of categories")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetOrdersAsync()
        {
            var result = await _orderViewModelBuilder.GetOrdersAsync();
            return Ok(result);
        }


        [Authorize]
        [HttpPut("/order")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "modification of a order")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PutOrderAsync([FromBody] OrderRequest model)
        {
            var result = await _orderViewModelBuilder.UpdateOrdersAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("/order")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "add a order")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> PostOrderAsync([FromBody] OrderRequest model)
        {
            var result = await _orderViewModelBuilder.AddOrdersAsync(model);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/order/{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string), Description = "Delete a order")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> DeleteOrderAsync([FromRoute] int id)
        {
            var result = await _orderViewModelBuilder.DeleteOrdersAsync(id);
            return Ok(result);
        }



        [HttpGet("/order/{orderId}/invoice")]
        public async Task<IActionResult> TelechargerPdf([FromRoute] int orderId, [FromServices] IWebHostEnvironment env)
        {
            var orderToGet = (await _invoiceViewModelBuilder.GetInvoicesAsync())
                .FirstOrDefault(i => i.OrderId == orderId);

            if (orderToGet == null || string.IsNullOrWhiteSpace(orderToGet.InvoiceLink))
                return NotFound();

            // Valeur venant de la BDD, ex: "wwwroot\\factures\\septembre_2025\\FA000000020.pdf"
            var link = orderToGet.InvoiceLink.Trim();

            // Normaliser les séparateurs pour l'OS courant
            var normalized = link.Replace('/', Path.DirectorySeparatorChar)
                                 .Replace('\\', Path.DirectorySeparatorChar);

            // Construire le chemin physique à partir de wwwroot
            // Si la BDD stocke déjà "wwwroot\...", on enlève "wwwroot" et on combine avec env.WebRootPath
            string relativeFromWebRoot = normalized.StartsWith("wwwroot" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                ? normalized.Substring(("wwwroot" + Path.DirectorySeparatorChar).Length)
                : normalized;

            var physicalPath = Path.Combine(env.WebRootPath, relativeFromWebRoot);

            if (!System.IO.File.Exists(physicalPath))
                return NotFound();

            var stream = new FileStream(physicalPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            // <- ICI on récupère juste le nom de fichier
            var fileName = Path.GetFileName(physicalPath); // "FA000000020.pdf"

            // (optionnel) exposer l’en-tête pour que le front puisse lire Content-Disposition
            Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");

            // Renvoie en téléchargement
            return new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = fileName
            };
        }

    }
}
