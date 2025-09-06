using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Builders.impl;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using MinshpWebApp.Domain.Services.Shipping;
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


        public ShippingController(IShippingProvider provider, IMemoryCache cache, IOrderRepository orders, IBoxalProviderViewModelBuilder boxalProviderViewModelBuilder)
        { _provider = provider; _cache = cache; _orders = orders; _boxalProviderViewModelBuilder = boxalProviderViewModelBuilder; }

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
        public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentCmd cmd)
        {
            int orderId = (int)cmd.Shipment.OrderId;
            var order = await _orders.GetByIdAsync(orderId);
            if (order is null) return NotFound();

            var s = await _provider.CreateShipmentAsync(cmd);

            order.BoxtalShipmentId = s.providerShipmentId;
            order.Carrier = s.carrier;
            order.ServiceCode = s.serviceCode;
            order.TrackingNumber = s.trackingNumber;
            order.LabelUrl = s.labelUrl;
            await _orders.UpdateOrdersAsync(order);

            return Ok(s);
        }

        // Optionnel: webhook Boxtal pour MAJ de statut
        [HttpPost("boxtal/webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement body)
        {
            var id = body.GetProperty("shipment_id").GetString()!;
            var status = body.GetProperty("status").GetString()!;
            var tracking = body.TryGetProperty("tracking", out var t) ? t.GetString() : null;

            var order = await _orders.FindByShipmentIdAsync(id);
            if (order != null)
            {
                order.Status = status switch
                {
                    "CREATED" => "Préparée",
                    "IN_TRANSIT" => "Expédiée",
                    "DELIVERED" => "Livrée",
                    "CANCELLED" => "Annulée",
                    _ => order.Status
                };
                if (!string.IsNullOrEmpty(tracking)) order.TrackingNumber = tracking;
                await _orders.UpdateOrdersAsync(order);
            }
            return Ok();
        }
    }
}
