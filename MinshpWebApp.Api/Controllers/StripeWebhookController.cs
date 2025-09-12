using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.Utils; // <<< alias explicite
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using MinshpWebApp.Domain.Services.Shipping;
using QuestPDF.Fluent;
using Stripe;
using Stripe;
using Stripe.Checkout;
using Stripe.Checkout;
using System.Globalization;
using static Microsoft.IO.RecyclableMemoryStreamManager;
using StripeEvent = Stripe.Event;
using StripeEvents = Stripe.Events;

namespace MinshpWebApp.Api.Controllers
{
    [ApiController]
    [Route("stripe/webhook")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly ILogger<StripeWebhookController> _logger;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly IOrderViewModelBuilder _ordersBuilder;
        private readonly IOrderCustomerProductViewModelBuilder _orderLinesBuilder; // <=== ajoute cette dépendance si pas encore
        private readonly IShippingProvider _shippingProvider;
        private readonly IOrderRepository _orderRepo;
        private readonly IInvoiceViewModelBuilder _invoiceBuilder;
        private readonly IProductViewModelBuilder _productBuilder;
        private readonly ITaxeViewModelBuilder _taxeBuilder;

        public StripeWebhookController(
            ILogger<StripeWebhookController> logger,
            IConfiguration config,
            IMemoryCache cache,
            IOrderViewModelBuilder ordersBuilder,
            IOrderCustomerProductViewModelBuilder orderLinesBuilder,
            IShippingProvider shippingProvider,
            IOrderRepository orderRepo,
            IInvoiceViewModelBuilder invoiceBuilder,
            IProductViewModelBuilder productBuilder,
            ITaxeViewModelBuilder taxeBuilder)
        {
            _logger = logger;
            _config = config;
            _cache = cache;
            _ordersBuilder = ordersBuilder;
            _orderLinesBuilder = orderLinesBuilder;
            _shippingProvider = shippingProvider;
            _orderRepo = orderRepo;
            _invoiceBuilder = invoiceBuilder;
            _productBuilder = productBuilder;
            _taxeBuilder = taxeBuilder;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var secret = _config["Stripe:WebhookSecret"];
            Event evt;

            try
            {
                evt = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    secret
                );
            }
            catch (StripeException e)
            {
                _logger.LogError("Stripe signature error: {Message}", e.Message);
                return BadRequest();
            }

            const string CheckoutSessionCompleted = "checkout.session.completed";
            if (!string.Equals(evt.Type, CheckoutSessionCompleted, StringComparison.OrdinalIgnoreCase))
            {
                return Ok();
            }

            var session = evt.Data.Object as Session;
            if (session is null) return Ok();

            var contextKey = session.Metadata.TryGetValue("contextKey", out var ctx) && !string.IsNullOrWhiteSpace(ctx)
                ? ctx
                : session.ClientReferenceId;

            if (string.IsNullOrWhiteSpace(contextKey))
            {
                _logger.LogWarning("Stripe webhook: contextKey manquant.");
                return Ok();
            }

            if (!_cache.TryGetValue(contextKey, out CheckoutSessionRequest ctxReq))
            {
                _logger.LogWarning("Stripe webhook: contexte {ContextKey} introuvable (cache expiré ?)", contextKey);
                return Ok();
            }

            // 1) créer la commande
            var totalTtc = (ctxReq.Amount / 100m);

            var addOrderReq = new OrderRequest
            {
                CustomerId = ctxReq.CustomerId,
                PaymentMethod = "Carte",
                Status = "En attente",
                Amount = ctxReq.Amount,
                DeliveryAmount = ctxReq.DeliveryAmount,
                DeliveryMode = ctxReq.DeliveryMode,
                Carrier = ctxReq.Carrier,
                ServiceCode = ctxReq.ServiceCode,
            };

            var orderIdStr = await _ordersBuilder.AddOrdersAsync(addOrderReq);
            int orderId = orderIdStr.Id;
            if (orderIdStr != null)
            {
                // si ton builder renvoie autre chose, adapte ici (ou change le type retour côté builder).
                _logger.LogInformation("AddOrdersAsync result: {Result}", orderId);
            }



            // 2) lignes de commande
            foreach (var it in ctxReq.OrderCustomerProducts)
            {
                var lineReq = new OrderCustomerProductRequest
                {
                    OrderId = orderId,
                    CustomerId = ctxReq.CustomerId,
                    ProductId = it.ProductId,
                    Quantity = it.Quantity,
                    ProductUnitPrice = it.ProductUnitPrice
                };
                await _orderLinesBuilder.AddOrderCustomerProductsAsync(lineReq);
            }

            // 3) création expédition via provider
            var cmd = new CreateShipmentV1Cmd
            {
                OperatorCode = ctxReq.OperatorCode,
                ServiceCode = ctxReq.ServiceCode,
                IsRelay = ctxReq.IsRelay,
                DropOffPointCode = ctxReq.DropOffPointCode,
                PickupPointCode = ctxReq.PickupPointCode,
                ContentDescription = "Objects high tech",
                DeclaredValue = ctxReq.DeclaredValue,
                Packages = ctxReq.Packages,
                ToType = "particulier",
                ToLastName = ctxReq.ToLastName,
                ToFirstName = ctxReq.ToFirstName,
                ToEmail = ctxReq.ToEmail,
                ToAddress = ctxReq.ToAddress,
                ToZip = ctxReq.ToZip,
                ToCity = ctxReq.ToCity,
                ToCivility = ctxReq.ToCivility,
                ToPhone = ctxReq.ToPhone,
                ToCountry = ctxReq.ToCountry ?? "FR",
                FromType = "entreprise",
                FromCivility = "M",
                FromCompany = "Mins Shop",
                FromLastName = "Camara",
                FromFirstName = "Minamba",
                FromEmail = "minamba.c@gmail.com",
                FromPhone = "+33624957558",
                FromAddress = "2 Rue jules vallès",
                FromZip = "91000",
                FromCity = "Evry-courcouronnes",
                FromCountry = "FR",
                TakeOverDate = DateTime.UtcNow.Date,
                ExternalOrderId = orderId.ToString(),
                OrderId = orderId
            };

            var ship = await _shippingProvider.CreateShipmentAsync(cmd);

            // 4) MAJ commande avec infos transport
            var order = await _orderRepo.GetByIdAsync(orderId.ToString());
            if (order != null)
            {
                order.BoxtalShipmentId = ship.providerShipmentId;
                order.Carrier = ship.carrier;
                order.ServiceCode = ship.serviceCode;
                order.TrackingNumber = ship.trackingNumber;
                order.LabelUrl = ship.labelUrl;
                await _orderRepo.UpdateOrdersAsync(order);
            }


            //5) Creation de la facture
            var newInvoice = new InvoiceRequest
            {
                OrderId = orderId,
                CustomerId = ctxReq.CustomerId,   
            };

            var newInvoiceToGet = await _invoiceBuilder.AddInvoicesAsync(newInvoice);


            var getInvoice = (await _invoiceBuilder.GetInvoicesAsync()).FirstOrDefault(i => i.Id == newInvoiceToGet.Id);

            var orderProductList = (await _orderLinesBuilder.GetOrderCustomerProductsAsync()).Where(ocp => ocp.OrderId == orderId && ocp.CustomerId == ctxReq.CustomerId).ToList();

            var tva = (await _taxeBuilder.GetTaxesAsync()).FirstOrDefault(t => t.Name == "TVA");
            var itemsInvoice = new List<Utils.InvoiceItem>();
            decimal totalHt = 0m;

            foreach (var item in orderProductList)
            {
                var getProduct = (await _productBuilder.GetProductsAsync()).FirstOrDefault(p => p.Id == item.ProductId);

                totalHt = (decimal)(totalHt + getProduct.Price);

                var it = new Utils.InvoiceItem
                {
                    Description = getProduct.Brand + " " + getProduct.Model,
                    Quantity = item.Quantity ?? 1,
                    UnitPrice = item.ProductUnitPrice ?? 0m
                };

                itemsInvoice.Add(it);
            }

            //6) Generation de la facture PDF et envoi par email
            var invoice = new InvoiceDocument
            {
                InvoiceNumber = getInvoice.InvoiceNumber,
                OrderNumber = orderIdStr.OrderNumber,
                InvoiceDate = (DateTime)getInvoice.DateCreation,
                BilledTo = "mr Camara Minamba\n4 Avenue du Général de Gaulle\n77500 CHELLES\nFRANCE",
                ShippedTo = "mr Camara Minamba\nrue André Lalande\n91000 EVRY\nFRANCE",
                Items = itemsInvoice,
                TotalHT = totalHt,
                TVA = (decimal)tva.Purcentage
            };

            //7 Généreration de la facture et placement dans le dossier du mois correspondant + maj de l'URL dans la facture
            var invoiceLink = FactureHelper.SaveInvoicePdf(invoice);

            await _invoiceBuilder.UpdateInvoicesAsync(new InvoiceRequest
            {
                Id = getInvoice.Id,
                InvoiceLink = invoiceLink
            });

            //7) nettoyage context cache
            _cache.Remove(contextKey);

            return Ok();
        }

        private static string CarrierToOperator(string? carrier, string? fallbackNetwork)
        {
            var s = (carrier ?? "").ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(fallbackNetwork) && fallbackNetwork!.Length == 4) return fallbackNetwork!;
            if (s.Contains("chrono")) return "CHRP";
            if (s.Contains("ups")) return "UPSE";
            if (s.Contains("mondial")) return "MONR";
            if (s.Contains("poste")) return "POFR";
            return fallbackNetwork ?? "CHRP";
        }
    }
}
