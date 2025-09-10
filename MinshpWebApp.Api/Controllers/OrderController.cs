using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MinshpWebApp.Api.Builders;
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
        IOrderViewModelBuilder _orderViewModelBuilder;
        private readonly IConfiguration _configuration;
        private readonly StripeSettings _stripe;
        private readonly string _frontendBaseUrl;


        public OrderController(IOrderViewModelBuilder orderViewModelBuilder, IOptions<StripeSettings> stripe, IConfiguration configuration)
        {
            _orderViewModelBuilder = orderViewModelBuilder ?? throw new ArgumentNullException(nameof(orderViewModelBuilder), $"Cannot instantiate {GetType().Name}");
            _stripe = stripe.Value ?? throw new ArgumentNullException(nameof(stripe), $"Cannot instantiate {GetType().Name}");
            _frontendBaseUrl = configuration["FrontendBaseUrl"];
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



        //STRIPE

        [HttpPost("/order/stripe")]
        public IActionResult CreateCheckoutSession([FromBody] StripeRequest request)
        {
            try
            {
                StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

                var options = new SessionCreateOptions
                {
                    Mode = "payment",
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = request.Currency ?? "eur",
                        UnitAmount = request.Amount, // ⚠️ déjà en centimes
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = string.IsNullOrWhiteSpace(request.Description) ? "Commande" : request.Description,
                        },
                    },
                    Quantity = 1,
                },
            },
                    SuccessUrl = $"{request.SuccessUrl}?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = request.CancelUrl,
                    CustomerEmail = request.CustomerEmail,
                    Metadata = request.Metadata
                };

                var service = new Stripe.Checkout.SessionService();
                var session = service.Create(options);

                // Variante 1: tu renvoies l'URL et tu rediriges côté front
                return Ok(new { sessionUrl = session.Url });

                // Variante 2 (si tu préfères redirectToCheckout côté front) :
                // return Ok(new { sessionId = session.Id, publishableKey = _configuration["Stripe:PublishableKey"] });
            }
            catch (StripeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        [HttpGet("/order/stripe/confirm")]
        public async Task<IActionResult> Confirm([FromQuery] string session_id)
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            var service = new Stripe.Checkout.SessionService();
            var session = await service.GetAsync(session_id, new Stripe.Checkout.SessionGetOptions
            {
                Expand = new List<string> { "payment_intent" }
            });

            var confirmed = string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase);
            return Ok(new
            {
                confirmed,
                amount = session.AmountTotal,
                currency = session.Currency
            });
        }






        public class CheckoutPayload
        {
            public long Amount { get; set; }        // in cents
            public string Currency { get; set; } = "eur";
            public int? CustomerId { get; set; }
            public string UserEmail { get; set; }

            public object Shipping { get; set; }    // on garde le JSON pour metadata
            public object Basket { get; set; }
            public object Shipment { get; set; }
        }

    }
}
