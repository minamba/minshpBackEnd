using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Builders.impl;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.Utils; // <<< alias explicite
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using MinshpWebApp.Domain.Services;
using MinshpWebApp.Domain.Services.impl;
using MinshpWebApp.Domain.Services.Shipping;
using QuestPDF.Fluent;
using Stripe;
using Stripe;
using Stripe.Checkout;
using Stripe.Checkout;
using System.Globalization;
using System.Linq;
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
        private readonly IApplicationViewModelBuilder _applicationBuilder;
        private readonly ICategoryService _categorynService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly ITaxeService _taxeService;
        private readonly IBillingAddressService _billingAddressService;
        private readonly IPromotionService _promotionService;
        private readonly IPromotionCodeService _promotionCodeService;
        private readonly IMailViewModelBuilder _mailViewModelBuilder;


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
            ITaxeViewModelBuilder taxeBuilder,
            IApplicationViewModelBuilder applicationBuilder,
            ICategoryService categorynService,
            ITaxeService taxeService,
            IBillingAddressService billingAddressService,
            IPromotionService promotionService,
            IPromotionCodeService promotionCodeService,
            ISubCategoryService subCategoryService,
            IMailViewModelBuilder mailViewModelBuilder)
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
            _applicationBuilder = applicationBuilder;
            _categorynService = categorynService;
            _taxeService = taxeService;
            _billingAddressService = billingAddressService;
            _promotionService = promotionService;
            _promotionCodeService = promotionCodeService;
            _subCategoryService = subCategoryService;
            _mailViewModelBuilder = mailViewModelBuilder;
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
            decimal totalHtWithTva = 0m;
            decimal? totalTaxes = 0m;
            int? tvaPurcentage = 0;
            decimal tvaAmount = 0;
            decimal? priceHt = 0;
            decimal? remise = 0;
            int TotalproductPromotionPurcentage = 0;
            decimal totalTTC = 0;
            decimal BasePriceWithRemise = 0;
            List<string> getTaxes = new List<string>();
            List<string> taxes = new List<string>();
            int countProductInCart = 0;

            var products = await _productBuilder.GetProductsAsync();
            var subCategories = await _subCategoryService.GetSubCategoriesAsync();
            var categories = await _categorynService.GetCategoriesAsync();


            foreach (var item in orderProductList)
            {
                countProductInCart++;
                var getProduct = products.FirstOrDefault(p => p.Id == item.ProductId);
                var getSubCategory = subCategories.FirstOrDefault(c => c.Id == getProduct.IdSubCategory);
                var getCategory = categories.FirstOrDefault(c => c.Name == getProduct.Category);

                    getTaxes.Add(getCategory.IdTaxe);

                if (getSubCategory != null)
                    getTaxes.Add(getSubCategory.IdTaxe);
      

                
                TotalproductPromotionPurcentage = await GetTotalPromotionPurcentageOfProduct(getProduct, getCategory);


                foreach (var tx in getTaxes)
                {
                    var list = tx.Split(',').ToList();

                    foreach (var l in list)
                    {
                        taxes.Add(l);
                    }
                }

                foreach (var t in taxes)
                {
                    var tax = (await _taxeService.GetTaxesAsync()).FirstOrDefault(ta => ta.Id == int.Parse(t));
                    if (tax.Name.ToLower() != "tva")
                        totalTaxes +=  tax.Amount;
                    else
                        tvaPurcentage = tax.Purcentage;
                }

                totalTaxes = totalTaxes * item.Quantity;


                //calcul du prix hors taxe ainsi que du montant de la remise ***************
                if(TotalproductPromotionPurcentage != 0)
                    BasePriceWithRemise = (decimal)((getProduct.Price) - (getProduct.Price * (TotalproductPromotionPurcentage / 100m)));
                else
                    BasePriceWithRemise = (decimal)getProduct.Price;


                remise = (decimal)getProduct.Price - BasePriceWithRemise;
                totalHt = (decimal)((decimal)((totalHt + getProduct.Price) * item.Quantity) - remise);
                tvaAmount = (decimal)(totalHt * (tvaPurcentage / 100m));
                totalHtWithTva = totalHt + tvaAmount;

                totalTTC = (decimal)(totalHtWithTva + totalTaxes + ctxReq.DeliveryAmount); //j'evite d'arrondir au supérieur pour que le montant soit exactement le meme que sur la commande
                
                //si le total TTC calculé est différent de celui de la commande, c'est que un code promo a été appliqué en dernière minute. donc on va ajuster TotalproductPromotionPurcentage
                if (countProductInCart == orderProductList.Count && totalTTC != ctxReq.Amount)
                {
                    //je dois tout remettre a zero pour recalculer proprement car les valeurs sont interdependantes
                    totalTTC = 0;
                    BasePriceWithRemise = 0;
                    remise = 0;
                    totalHt = 0;
                    tvaAmount = 0;
                    totalHtWithTva = 0;
                    TotalproductPromotionPurcentage = 0;

                    foreach (var i in orderProductList)
                    {
                        var useCodes = ctxReq.UseCodes;
                        var getProd = products.FirstOrDefault(p => p.Id == i.ProductId);
                        var getCa = categories.FirstOrDefault(c => c.Name == getProd.Category);

                        TotalproductPromotionPurcentage = await GetTotalPromotionPurcentageOfProduct(getProd, getCa);
                        var promotionCodePurcentage = (await GetPromotionCodePurcentageOfProduct(getProd, useCodes));

                        if(promotionCodePurcentage != 0)
                            TotalproductPromotionPurcentage += promotionCodePurcentage;


                        if (TotalproductPromotionPurcentage != 0)
                            BasePriceWithRemise = (decimal)((getProd.Price) - (getProd.Price * (TotalproductPromotionPurcentage / 100m)));
                        else
                            BasePriceWithRemise = (decimal)getProd.Price;

                        remise = (decimal)getProd.Price - BasePriceWithRemise;
                        totalHt = (decimal)((decimal)((totalHt + getProd.Price) * item.Quantity) - remise);
                        tvaAmount = (decimal)(totalHt * (tvaPurcentage / 100m));
                        totalHtWithTva = totalHt + tvaAmount;

                        totalTTC = (decimal)(totalHtWithTva + totalTaxes + ctxReq.DeliveryAmount);
                    }
                }

                var it = new Utils.InvoiceItem
                {
                    Description = getProduct.Brand + " " + getProduct.Model,
                    Quantity = item.Quantity ?? 1,
                    UnitPrice = getProduct.Price ?? 0m,
                    Reduction = (decimal)(remise * item.Quantity),
                    TotalPriceItemProduct = (decimal)((decimal)(getProduct.Price * item.Quantity) - (remise * item.Quantity))
                };

                itemsInvoice.Add(it);

                 getTaxes.Clear() ;
                 taxes.Clear();
            }

            var deliveryItem = new Utils.InvoiceItem
            {
                Description = "Frais de livraison",
                TotalPriceItemProduct = (decimal)ctxReq.DeliveryAmount
            };

            itemsInvoice.Add(deliveryItem);

            var billingAddress = (await _billingAddressService.GetBillingAddressesAsync()).FirstOrDefault(b => b.IdCustomer == ctxReq.CustomerId);





            //6) Generation de la facture PDF et envoi par email

            var applicationSettings = (await _applicationBuilder.GetApplicationAsync()).FirstOrDefault();
            var invoice = new InvoiceDocument
            {
                SocietyName = applicationSettings.SocietyName,
                SocietyAddress = applicationSettings.SocietyAddress,
                SocietyZipCode = applicationSettings.SocietyZipCode,
                SocietyCity = applicationSettings.SocietyCity,
                EcoPart = (decimal)totalTaxes,
                InvoiceNumber = getInvoice.InvoiceNumber,
                OrderNumber = orderIdStr.OrderNumber,
                InvoiceDate = (DateTime)getInvoice.DateCreation,
                BilledTo = $"{ctxReq.ToCivility} {ctxReq.ToLastName} {ctxReq.ToFirstName}\n{ctxReq.ToAddress}\n{ctxReq.ToZip} {ctxReq.ToCity}\n{ctxReq.ToCountry}",
                ShippedTo = $"{ctxReq.ToCivility} {ctxReq.ToLastName} {ctxReq.ToFirstName}\n{billingAddress.Address}\n{billingAddress.PostalCode} {billingAddress.City}\n{billingAddress.Country}",
                Items = itemsInvoice,
                TotalHT = (decimal)(totalHt + ctxReq.DeliveryAmount),
                TVAPurcentage = (decimal)tvaPurcentage,
                TVA = tvaAmount,
                TotalTTC = totalTTC
            };

            //7 Généreration de la facture et placement dans le dossier du mois correspondant + maj de l'URL dans la facture
            var invoiceLink = FactureHelper.SaveInvoicePdf(invoice);

            await _invoiceBuilder.UpdateInvoicesAsync(new InvoiceRequest
            {
                Id = getInvoice.Id,
                InvoiceLink = invoiceLink
            });


            //8 Envoie de l'e-mail
            await _mailViewModelBuilder.SendMailRegistration("minamba.c@gmail.com");


            //9) nettoyage context cache
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


        private async Task<int> GetTotalPromotionPurcentageOfProduct(ProductVIewModel product, Domain.Models.Category category)
        {
            int? TotalproductPromotionPurcentage = 0;
            int? productPromotionPurcentage = 0;
            int? productPromotionCategoryPurcentage = 0;
            int? productPromotionSubCategoryPurcentage = 0;


            var getProductPromotion = (await _promotionService.GetPromotionsAsync()).FirstOrDefault(p => p.IdProduct == product.Id);
            var getProductCategoryPromotion = (await _promotionCodeService.GetPromotionCodesAsync()).FirstOrDefault(c => c.Id == category.IdPromotionCode);
            var getProductSubCategory = (await _subCategoryService.GetSubCategoriesAsync()).FirstOrDefault(sb => sb.Id == product.IdSubCategory);

            if (getProductPromotion != null)
            {
                if (getProductPromotion.EndDate >= DateTime.Now)
                    TotalproductPromotionPurcentage += getProductPromotion.Purcentage;
            }

            if (getProductCategoryPromotion != null)
            {
                if (getProductCategoryPromotion.EndDate >= DateTime.Now)
                    TotalproductPromotionPurcentage += getProductCategoryPromotion.Purcentage;
            }

            if (getProductSubCategory != null)
            {
                var getProductSubCategoryPromotion = (await _promotionCodeService.GetPromotionCodesAsync()).FirstOrDefault(c => c.Id == getProductSubCategory.IdPromotionCode);

                if (getProductSubCategoryPromotion != null)
                {
                    if (getProductSubCategoryPromotion.EndDate >= DateTime.Now)
                        TotalproductPromotionPurcentage += getProductSubCategoryPromotion.Purcentage;
                }
            }


            return (int)TotalproductPromotionPurcentage;
        }


        private async Task<int> GetPromotionCodePurcentageOfProduct(ProductVIewModel product, List<PromoUseCode> useCodes)
        {

             var getProductPromotionCodePurcentage = (await _promotionCodeService.GetPromotionCodesAsync()).FirstOrDefault(p => p.Id == product.IdPromotionCode);

            if (getProductPromotionCodePurcentage != null)
            {
                foreach (var uc in useCodes)
                {
                    if (uc.Name.ToLower().Contains(getProductPromotionCodePurcentage.Name.ToLower()))
                            return (int)getProductPromotionCodePurcentage.Purcentage;
                }

            }

            return 0;
        }
    }
}
