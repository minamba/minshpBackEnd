using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MinshpWebApp.Api.Options;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.Utils;
using QuestPDF.Fluent;
using Stripe;
using Stripe.Checkout;

namespace MinshpWebApp.Api.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly string _frontendBaseUrl;

        private readonly StripeClient _client;

        public PaymentController(
            IConfiguration config,
            IMemoryCache cache,
            StripeClient client // doit être injecté avec la clé secrète
        )
        {
            _config = config;                     // ✅ manquait
            _cache = cache;

            _frontendBaseUrl = _config["Stripe:FrontendBaseUrl"];

            var secret = _config["Stripe:SecretKey"];
            StripeConfiguration.ApiKey = secret;  // utile pour API utilitaires
            _client = client ?? new StripeClient(secret);
        }

        [HttpPost("checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutSessionRequest req)
        {

            // 1) URLs
            var successUrl = !string.IsNullOrWhiteSpace(req.SuccessUrl)
                ? req.SuccessUrl
                : $"{_frontendBaseUrl}/success?session_id={{CHECKOUT_SESSION_ID}}";

            var cancelUrl = !string.IsNullOrWhiteSpace(req.CancelUrl)
                ? req.CancelUrl
                : $"{_frontendBaseUrl}/cancel";

            // 2) Contexte volumineux en cache (2h)
            var contextKey = $"ctx_{Guid.NewGuid():N}";
            _cache.Set(contextKey, req, TimeSpan.FromHours(2));

            // 3) Montant -> centimes (Stripe)
            var amountCents = (long)Math.Round((req.Amount ?? 0m) * 100m);

            // 4) Création de la session
            var options = new SessionCreateOptions
            {
                Mode = "payment",
                CustomerEmail = req.ToEmail,
                ClientReferenceId = contextKey,
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Quantity = 1,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "eur",
                            UnitAmount = amountCents, // ✅ centimes
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Commande Mins Shop"
                            }
                        }
                    }
                },
                Metadata = new Dictionary<string, string>
                {
                    ["contextKey"] = contextKey,
                    ["customerId"] = req.CustomerId?.ToString() ?? "",
                    ["shippingMode"] = req.DeliveryMode ?? "home",
                    ["rateCode"] = req.ServiceCode ?? "",
                    ["carrier"] = req.Carrier ?? ""
                }
            };

            var service = new SessionService(_client);         // ✅ client configuré
            var session = await service.CreateAsync(options);  // ✅ méthode async

            return Ok(new
            {
                sessionId = session.Id,
                publishableKey = _config["Stripe:PublishableKey"]
            });
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> Confirm([FromQuery] string session_id)
        {
            var service = new SessionService(_client); // ✅ pas de “No API key…”
            var session = await service.GetAsync(session_id, new SessionGetOptions
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

        //POUR TESTER LA CREATION DE PDF
        //[HttpGet("test-pdf")]
        //public IActionResult TestPdf()
        //{
        //    var dossier = Path.Combine("wwwroot", "factures", "test");
        //    Directory.CreateDirectory(dossier);

        //    var chemin = Path.Combine(dossier, "test-pdf.pdf");

        //    var document = new TestPdf();

        //    try
        //    {
        //        QuestPDF.Fluent.Document.Create(c => document.Compose(c))
        //                                .GeneratePdf(chemin);

        //        Console.WriteLine("✅ PDF écrit : " + chemin);
        //        return Ok("PDF OK");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("❌ Erreur : " + ex.Message);
        //        return StatusCode(500, ex.Message);
        //    }
        //}
    }
}
