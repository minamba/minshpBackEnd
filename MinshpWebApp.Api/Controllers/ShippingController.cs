using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Builders.impl;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using MinshpWebApp.Domain.Services.Shipping;
using MinshpWebApp.Domain.Services.Shipping.impl;
using System.Text.Json;

namespace MinshpWebApp.Api.Controllers
{

    [ApiController]
    [Route("Shippings")]
    public class ShippingController : ControllerBase
    {
        private readonly IShippingProvider _provider;
        private readonly IMemoryCache _cache;
        private readonly IOrderRepository _orders;
        private readonly IBoxalProviderViewModelBuilder _boxalProviderViewModelBuilder;
        private readonly BoxtalOptions _boxtal;


        public ShippingController(IShippingProvider provider, IMemoryCache cache, IOrderRepository orders, IBoxalProviderViewModelBuilder boxalProviderViewModelBuilder, IOptions<BoxtalOptions> boxtal)
        { _provider = provider; 
            _cache = cache; 
            _orders = orders; 
            _boxalProviderViewModelBuilder = boxalProviderViewModelBuilder; 
            _boxtal = boxtal.Value; 
        }

        [HttpPost("rates")]
        public async Task<IActionResult> GetRates([FromBody] OrderDetailsRequest request)
        {
                var rates = await _boxalProviderViewModelBuilder.GetRatesAsync(request);
            
            return Ok(rates);
        }

        [HttpGet("relays")]
        public async Task<IActionResult> GetRelays([FromQuery] string zip, [FromQuery] string country = "FR", [FromQuery] int limit = 20)
        {
            var key = $"relays:{zip}:{country}:{limit}";
            if (!_cache.TryGetValue(key, out List<Relay>? relays))
            {
                relays = await _provider.GetRelaysAsync(zip, country, limit);
                _cache.Set(key, relays, TimeSpan.FromMinutes(10));
            }
            return Ok(relays);
        }


        [HttpGet("relays/by-address")]
        public async Task<IActionResult> GetRelaysByAddress(
        [FromQuery] string? number,
        [FromQuery] string? street,
        [FromQuery] string? city,
        [FromQuery] string? postalCode,
        [FromQuery] string? state,
        [FromQuery] string countryIsoCode = "FR",
        [FromQuery] string[]? searchNetworks = null,
        [FromQuery] int limit = 30)
        {
            var key = $"relays:addr:{number}|{street}|{city}|{postalCode}|{state}|{countryIsoCode}|{string.Join(",", searchNetworks ?? Array.Empty<string>())}|{limit}";
            if (!_cache.TryGetValue(key, out List<RelaysAddressViewModel>? relays))
            {
                var q = new RelaysAddressRequest
                {
                    Number = number,
                    Street = street,
                    City = city,
                    PostalCode = postalCode,
                    State = state,
                    CountryIsoCode = countryIsoCode,
                    SearchNetworks = searchNetworks,
                    Limit = limit
                };
                relays = await _boxalProviderViewModelBuilder.GetRelaysByAddressAsync(q);
                _cache.Set(key, relays, TimeSpan.FromMinutes(10));
            }
            return Ok(relays);
        }



        [HttpGet("contentCodes")]
        public async Task<IActionResult> GetContentCategoriesAsync()
        {
            var key = $"content";
            if (!_cache.TryGetValue(key, out CodeCategoriesViewModel? codes))
            {
                codes = await _boxalProviderViewModelBuilder.GetContentCategoriesAsync();
                _cache.Set(key, codes, TimeSpan.FromMinutes(10));
            }
            return Ok(codes);
        }



        [HttpPost("create-shipment")]
        public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentV1Cmd cmd)
        {
            // --- 1) ExternalOrderId valide ---
            if (string.IsNullOrWhiteSpace(cmd.ExternalOrderId))
                return BadRequest("ExternalOrderId manquant ou invalide.");

            var orderId = cmd.ExternalOrderId;

            var order = await _orders.GetByIdAsync(orderId.ToString());
            if (order is null) return NotFound($"Commande {orderId} introuvable.");

            //var internalNumberForOrder = order.OrderNumber;

            // --- 2) url_push requis par V1 ---
            if (!string.IsNullOrWhiteSpace(_boxtal.V1PushUrlBase))
            {
                var pushUrl = QueryHelpers.AddQueryString(_boxtal.V1PushUrlBase, new Dictionary<string, string?>
                {
                    ["orderId"] = orderId,
                    ["token"] = _boxtal.V1PushToken
                });

                // si tu as ajouté UrlPush au DTO
                cmd.UrlPush = pushUrl;
            }


            // --- 3) Shop-to-shop : depot.pointrelais (fallback) ---
            //var isShopToShop = (cmd.OperatorCode?.Equals("CHRP", StringComparison.OrdinalIgnoreCase) ?? false)
            //                   && (cmd.ServiceCode?.Contains("shop", StringComparison.OrdinalIgnoreCase) ?? false);
            //if (isShopToShop && string.IsNullOrWhiteSpace(cmd.DropOffPointCode) && !string.IsNullOrWhiteSpace(_boxtal.DefaultDropOffPointCode))
            //{
            //    cmd.DropOffPointCode = _boxtal.DefaultDropOffPointCode;
            //}




            // --- 4) Appel provider ---
            var s = await _provider.CreateShipmentAsync(cmd);

            // --- 5) MAJ commande ---
            order.BoxtalShipmentId = s.providerShipmentId;
            order.Carrier = s.carrier;
            order.ServiceCode = s.serviceCode;
            order.TrackingNumber = s.trackingNumber;
            order.LabelUrl = s.labelUrl;
            await _orders.UpdateOrdersAsync(order);

            return Ok(s);
        }

        // Optionnel: webhook Boxtal pour MAJ de statut
        // ====== Webhook Boxtal V1 (GET/POST + query/form) ======
        [AllowAnonymous]
        [Consumes("application/x-www-form-urlencoded", "application/json")]
        [HttpGet("boxtal/webhook")]
        [HttpPost("boxtal/webhook")]
        public async Task<IActionResult> Webhook()
        {
            // 1) Sécurité : jeton en query/form
            var token = Request.Query["token"].FirstOrDefault()
                        ?? (Request.HasFormContentType ? Request.Form["token"].FirstOrDefault() : null);

            if (!string.Equals(token, _boxtal.V1PushToken, StringComparison.Ordinal))
                return Unauthorized();

            // Petit helper pour piocher indifféremment dans query/form
            string Get(string key, params string[] aliases)
            {
                var v = Request.Query[key].FirstOrDefault()
                    ?? (Request.HasFormContentType ? Request.Form[key].FirstOrDefault() : null);
                if (!string.IsNullOrEmpty(v)) return v;

                foreach (var a in aliases)
                {
                    v = Request.Query[a].FirstOrDefault()
                        ?? (Request.HasFormContentType ? Request.Form[a].FirstOrDefault() : null);
                    if (!string.IsNullOrEmpty(v)) return v;
                }
                return null!;
            }

            // 2) Champs connus (+ quelques alias courants des V1)
            var orderId = Get("orderId", "external_id", "order_id");
            var shipmentId = Get("emc_reference", "envoi", "id", "shipmentId", "expedition_id");
            var tracking = Get("carrier_reference");
            var labelUrl = Get("labelUrl", "label_url", "etiquette_url", "url_etiquette");
            var status = Get("text","etat", "state");

            // 3) Fallback JSON (si Boxtal poste en JSON)
            if (string.IsNullOrEmpty(status)
                && !string.IsNullOrEmpty(Request.ContentType)
                && Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    using var doc = await JsonDocument.ParseAsync(Request.Body);
                    var root = doc.RootElement;

                    orderId ??= root.TryGetProperty("orderId", out var o) ? o.GetString() :
                                   root.TryGetProperty("external_id", out o) ? o.GetString() : null;

                    shipmentId ??= root.Val("emc_reference", "shipment_id", "id", "shipmentId", "expedition_id", "envoi");
                    tracking ??= root.Val("carrier_reference");
                    labelUrl ??= root.Val("labelUrl", "label_url", "etiquette_url", "url_etiquette");
                    status ??= root.Val("text", "status", "state", "etat");
                }
                catch { /* on ignore pour ne pas casser le 200 */ }
            }

            // 4) MAJ commande (fail-soft, réponse 200 quand même)
            try
            {
                if (int.TryParse(orderId, out var oid))
                {
                    var order = await _orders.GetByIdAsync(oid.ToString());
                    if (order != null)
                    {
                        if (!string.IsNullOrWhiteSpace(shipmentId)) order.BoxtalShipmentId = shipmentId;
                        if (!string.IsNullOrWhiteSpace(tracking)) order.TrackingNumber = tracking;
                        if (!string.IsNullOrWhiteSpace(labelUrl)) order.LabelUrl = labelUrl;

                        var up = (status ?? "").Trim().ToUpperInvariant();
                        if (!string.IsNullOrEmpty(up))
                        {
                            order.Status = up switch
                            {
                                "CREATED" => "Préparée",
                                "IN_TRANSIT" => "Expédiée",
                                "DELIVERED" => "Livrée",
                                "CANCELLED" => "Annulée",
                                _ => status
                            };
                        }
                        await Task.Delay(2000); // 200
                        await _orders.UpdateOrdersAsync(order);
                    }
                }
            }
            catch
            {
                // log si tu veux, mais ne renvoie pas 5xx pour éviter les retries en boucle
            }

            return Ok("OK");
        }
    }


        // petit helper d’extension pour JSON
        file static class JsonElExt
        {
            public static string? Val(this JsonElement el, params string[] names)
            {
                foreach (var n in names)
                    if (el.TryGetProperty(n, out var v))
                        return v.GetString();
                return null;
            }
        }
}
