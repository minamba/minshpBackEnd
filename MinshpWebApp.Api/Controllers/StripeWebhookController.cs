using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Builders.impl;
using Stripe;

namespace MinshpWebApp.Api.Controllers
{
    [ApiController]
    [Route("StripeWebhook")]
    public class StripeWebhookController : ControllerBase
    {
        private IOrderViewModelBuilder _order;
        //IMailViewModelBuilder _mail;
        private readonly ILogger<StripeWebhookController> _logger;
        private readonly IConfiguration _configuration;
        private bool isMailSent = false;

        public StripeWebhookController(ILogger<StripeWebhookController> logger, IConfiguration configuration, IOrderViewModelBuilder order /*, IMailViewModelBuilder mail*/)
        {
            _logger = logger;
            _configuration = configuration;
            _order = order;
            //_mail = mail;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var secret = _configuration["Stripe:WebhookSecret"];

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    secret
                );
            }
            catch (StripeException e)
            {
                _logger.LogError("❌ Erreur vérification Stripe : " + e.Message);
                return BadRequest();
            }

            Console.WriteLine("📩 Stripe Event Type reçu : " + stripeEvent.Type);

            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                try
                {
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                    // 🔍 Log complet des metadata
                    var metaLog = string.Join(", ", session.Metadata.Select(kv => $"{kv.Key}={kv.Value}"));
                    _logger.LogInformation($"📦 Metadata Stripe : {metaLog}");

                    // ✅ Lecture sécurisée des metadata
                    session.Metadata.TryGetValue("firstName", out var firstName);
                    session.Metadata.TryGetValue("lastName", out var lastName);
                    session.Metadata.TryGetValue("phoneNumber", out var phoneNumber);
                    session.Metadata.TryGetValue("mail", out var mail);
                    session.Metadata.TryGetValue("amount", out var amount);
                    session.Metadata.TryGetValue("date", out var date);
                    session.Metadata.TryGetValue("paymentmode", out var paymentmode);
                    session.Metadata.TryGetValue("idSeminaire", out var idSeminaire);
                    session.Metadata.TryGetValue("title", out var seminaireTitle);


                    //// ✅ Envoi du mail
                    //var mailSent = await _mail.SendMailPayment(new Requests.PaymentRequest
                    //{
                    //    Recipient = mail,
                    //    SeminaireTitle = seminaireTitle
                    //});

                    //if (mailSent != null)
                    //    isMailSent = true;
                    //else
                    //    isMailSent = false;


                    //// ✅ Enregistrement dans la base
                    //var result = await _payment.AddPayment(new Payment
                    //{
                    //    FirstName = firstName,
                    //    LastName = lastName,
                    //    Mail = mail,
                    //    PhoneNumber = phoneNumber,
                    //    Amount = session.AmountTotal / 100.0m,
                    //    Date = DateTime.UtcNow,
                    //    PaymentMode = "CB",
                    //    IdSeminaire = int.TryParse(idSeminaire, out var id) ? id : 0,
                    //    MailSent = isMailSent
                    //});

                    //Console.WriteLine($"✅ Paiement ajouté : {result}");

                    //_logger.LogInformation($"✅ Paiement confirmé et mail envoyé à {mail}");
                }
                catch (Exception ex)
                {
                    _logger.LogError("❌ Erreur dans le traitement du webhook Stripe : " + ex.Message);
                    return StatusCode(500, "Erreur interne lors du traitement du paiement.");
                }
            }

            return Ok();
        }
    }
}
