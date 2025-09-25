using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Builders.impl;
using MinshpWebApp.Api.Options;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.Utils;
using QuestPDF.Fluent;
using Stripe;
using Stripe.Checkout;

namespace MinshpWebApp.Api.Controllers
{
    public record FulfillmentResult(bool Success, int OrderId, string? Error = null);

    [ApiController]
    [Route("payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly ITelegramViewModelBuilder _telegramViewModelBuilder;
        private readonly string _frontendBaseUrl;

        private readonly StripeClient _client;

        public PaymentController(IConfiguration config,IMemoryCache cache,StripeClient client,ITelegramViewModelBuilder telegramViewModelBuilder)
        {
            _config = config;                     // ✅ manquait
            _cache = cache;
            _telegramViewModelBuilder = telegramViewModelBuilder;


            _frontendBaseUrl = _config["Stripe:FrontendBaseUrl"];

            var secret = _config["Stripe:SecretKey"];
            StripeConfiguration.ApiKey = secret;  // utile pour API utilitaires
            _client = client ?? new StripeClient(secret);
        }

        [HttpPost("checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutSessionRequest req)
        {
            // 1) success -> callback serveur (PAS la page front)
            var apiBase = _config["Application:ApiBaseUrl"] ?? _config["App:ApiBaseUrl"];
            var successUrl = $"{apiBase!.TrimEnd('/')}/payments/callback?session_id={{CHECKOUT_SESSION_ID}}";

            // 2) cancel -> page front "cancel"
            var cancelUrl = !string.IsNullOrWhiteSpace(req.CancelUrl)
                ? req.CancelUrl
                : $"{_frontendBaseUrl.TrimEnd('/')}/cancel";

            // 3) Contexte volumineux en cache (2h) — clé uniforme
            var contextKey = $"ctx_{Guid.NewGuid():N}";
            var cacheKey = $"ctx:{contextKey}";
            _cache.Set(cacheKey, req, TimeSpan.FromHours(2));   // <-- une seule écriture avec la clé canonique

            // 4) Montant -> centimes (Stripe)
            var amountCents = (long)Math.Round((req.Amount ?? 0m) * 100m);

            // 5) Création de la session
            var options = new SessionCreateOptions
            {
                Mode = "payment",
                CustomerEmail = req.Shipment?.ToAddress?.Contact?.Email,
                ClientReferenceId = contextKey,     // <-- on envoie le contextKey à Stripe
                SuccessUrl = successUrl,            // <-- callback serveur
                CancelUrl = cancelUrl,             // <-- front
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                Quantity = 1,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "eur",
                    UnitAmount = amountCents,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Commande Mins Shop"
                    }
                }
            }
        },
                Metadata = new Dictionary<string, string>
                {
                    ["contextKey"] = contextKey,                              // <-- idem dans metadata
                    ["customerId"] = req.CustomerId?.ToString() ?? "",
                    ["shippingMode"] = req.DeliveryMode ?? "home",
                    ["rateCode"] = req.ServiceCode ?? "",
                    ["carrier"] = req.Carrier ?? "",
                }
            };

            var service = new SessionService(_client);
            var session = await service.CreateAsync(options);

            return Ok(new
            {
                sessionId = session.Id,
                publishableKey = _config["Stripe:PublishableKey"]
            });
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> Confirm([FromQuery] string session_id, CancellationToken ct)
        {
            var service = new SessionService(_client);
            var session = await service.GetAsync(session_id);

            var contextKey = session.Metadata != null && session.Metadata.TryGetValue("contextKey", out var k) && !string.IsNullOrWhiteSpace(k)
                ? k
                : session.ClientReferenceId;

            var resultKey = $"done:{contextKey}";

            // Option: on attend jusqu'à 20s que le webhook finisse (petit poll serveur)
            var deadline = DateTime.UtcNow.AddSeconds(20);
            while (DateTime.UtcNow < deadline)
            {
                if (_cache.TryGetValue(resultKey, out FulfillmentResult res))
                {
                    return Ok(new
                    {
                        confirmed = true,          // paiement Stripe ok
                        processed = true,          // traitement webhook terminé
                        ok = res.Success,          // vrai succès métier
                        orderId = res.OrderId,
                        error = res.Error
                    });
                }
                await Task.Delay(500, ct);
            }

            // Le paiement est ok mais le webhook n'a pas encore fini
            return Ok(new { confirmed = true, processed = false });
        }


        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string session_id, CancellationToken ct)
        {
            // 1) Récupère la session Stripe
            var service = new SessionService(_client);
            var session = await service.GetAsync(session_id, cancellationToken: ct);

            // 2) Retrouve la clé de contexte (envoyée à Stripe lors de la création)
            var contextKey =
                (session.Metadata != null &&
                 session.Metadata.TryGetValue("contextKey", out var ctx) &&
                 !string.IsNullOrWhiteSpace(ctx))
                ? ctx
                : session.ClientReferenceId;

            var resultKey = $"done:{contextKey}";
            var cacheKey = $"ctx:{contextKey}";

            // 3) Attendre que le webhook ait fini (poll léger)
            var deadline = DateTime.UtcNow.AddSeconds(10);
            while (DateTime.UtcNow < deadline && !_cache.TryGetValue(resultKey, out FulfillmentResult _))
            {
                await Task.Delay(500, ct);
            }

            var frontBase = _frontendBaseUrl.TrimEnd('/');

            // 4) Reprendre la request originale (si le cache est dispo)
            var req = _cache.Get<CheckoutSessionRequest>(cacheKey);

            // 4.b) Fallbacks depuis la session Stripe si le cache est manquant/expiré
            var emailFallback = session.CustomerDetails?.Email ?? session.CustomerEmail;
            var amountFallback = (session.AmountTotal ?? 0) / 100m; // AmountTotal en centimes
            var orderNumberMeta = session.Metadata?.GetValueOrDefault("orderNumber");

            // 5) Si le webhook a posé un résultat, on redirige selon succès/échec
            if (_cache.TryGetValue(resultKey, out FulfillmentResult res))
            {
                if (res.Success)
                {
                    return Redirect($"{frontBase}/success?session_id={session_id}&order={res.OrderId}");
                }
                else
                {
                    // Prévenir par Telegram avec des valeurs sûres (fallback si req == null)
                    await _telegramViewModelBuilder.SendErrorMessage(new TelegramRequest
                    {
                        Mail = req?.Shipment?.ToAddress?.Contact?.Email ?? emailFallback ?? "-",
                        ClientNumber = req?.CustomerNumber,
                        OrderAmount = req.Amount,
                        Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    });

                    return Redirect($"{frontBase}/error");
                }
            }

            // 6) Timeout : le webhook n'a pas fini -> on informe et on redirige
            if (req != null)
            {
                await _telegramViewModelBuilder.SendErrorMessage(new TelegramRequest
                {
                    Mail = req?.Shipment?.ToAddress?.Contact?.Email ?? emailFallback ?? "-",
                    ClientNumber = req?.CustomerNumber,
                    OrderAmount = req.Amount,
                    Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                });
            }

            return Redirect($"{frontBase}/error?reason=timeout");
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
