using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.Utils;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using MinshpWebApp.Domain.Services.Shipping;
using MinshpWebApp.Domain.Services.Shipping.impl;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Cryptography;
using System.Text;
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
        private readonly ILogger<ShippingController> _logger;

        public ShippingController(
            IShippingProvider provider,
            IMemoryCache cache,
            IOrderRepository orders,
            IBoxalProviderViewModelBuilder boxalProviderViewModelBuilder,
            IOptions<BoxtalOptions> boxtal,
            ILogger<ShippingController> logger)
        {
            _provider = provider;
            _cache = cache;
            _orders = orders;
            _boxalProviderViewModelBuilder = boxalProviderViewModelBuilder;
            _boxtal = boxtal.Value;
            _logger = logger;
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
            const string key = "content";
            if (!_cache.TryGetValue(key, out CodeCategoriesViewModel? codes))
            {
                codes = await _boxalProviderViewModelBuilder.GetContentCategoriesAsync();
                _cache.Set(key, codes, TimeSpan.FromMinutes(10));
            }
            return Ok(codes);
        }



        [HttpGet("subscription")]
        public async Task<IActionResult> GetSubscriptionAsync()
        {
            const string key = "content";
            if (!_cache.TryGetValue(key, out LiveTracking? tracking))
            {
                tracking = await _boxalProviderViewModelBuilder.GetSubscriptionAsync();
                _cache.Set(key, tracking, TimeSpan.FromMinutes(10));
            }
            return Ok(tracking);
        }


        [HttpPost("create-shipment")]
        public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentCmd cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd.ExternalId))
                return BadRequest("ExternalOrderId manquant ou invalide.");

            var order = await _orders.GetByIdAsync(cmd.ExternalId.ToString());
            if (order is null) return NotFound($"Commande {cmd.ExternalId} introuvable.");

            // url_push V1 (legacy)
            if (!string.IsNullOrWhiteSpace(_boxtal.V1PushUrlBase))
            {
                _ = QueryHelpers.AddQueryString(_boxtal.V1PushUrlBase, new Dictionary<string, string?>
                {
                    ["orderId"] = cmd.ExternalId,
                    ["token"] = _boxtal.V1PushToken
                });
            }

            var s = await _provider.CreateShipmentAsync(cmd);

            order.BoxtalShipmentId = s.ShipmentId;
            order.Carrier = cmd.OperatorCode;
            order.ServiceCode = s.ServiceCode;
            await _orders.UpdateOrdersAsync(order);

            return Ok(s);
        }

        // ================== V3: Souscription (génère le secret si manquant) ==================
        /// <summary>
        /// (Idempotent côté appli) Crée la souscription TRACKING_CHANGED.
        /// Si V3WebhookSecret ou V3CallbackUrl ne sont pas fournis dans la config,
        /// on les génère/construit en mémoire et on les retourne pour que tu les
        /// enregistres de manière PERSISTANTE (User Secrets / env vars).
        /// </summary>
        [HttpPost("boxtal/v3/subscribe")]
        public async Task<IActionResult> CreateV3Subscription(string evenType)
        {
            // 1) Callback par défaut si absent -> https://host/Shippings/boxtal/v3/webhook
            if (string.IsNullOrWhiteSpace(_boxtal.V3CallbackUrl))
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                _boxtal.V3CallbackUrl = $"{baseUrl}/Shippings/boxtal/v3/webhook";
            }

            // 2) Secret généré si manquant (32 octets -> 64 hex)
            string? generatedSecret = null;
            if (string.IsNullOrWhiteSpace(_boxtal.V3WebhookSecret))
            {
                generatedSecret = GenerateHexSecret(32);
                _boxtal.V3WebhookSecret = generatedSecret;
            }

            // 3) Appel provider (utilise _boxtal)
            var sub = await _boxalProviderViewModelBuilder.CreateSubscriptionAsync(evenType);

            // 4) Retourne la souscription + le secret (si on l’a généré)
            return Ok(new
            {
                subscription = sub,
                callbackUrl = _boxtal.V3CallbackUrl,
                // IMPORTANT : place cette valeur dans ta config persistante !
                generatedSecret
            });
        }

        // ================== V3: Webhook (HMAC SHA-256) ==================
        [AllowAnonymous]
        [HttpPost("boxtal/v3/webhook")]
        [Consumes("application/json")]
        public async Task<IActionResult> BoxtalV3Webhook()
        {
            // -- (idempotence + HMAC inchangés chez toi) --
            Request.EnableBuffering();
            string body;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8, false, leaveOpen: true))
                body = await reader.ReadToEndAsync();
            Request.Body.Position = 0;

            var providedSig = Request.Headers["X-Bxt-Signature"].FirstOrDefault()
                           ?? Request.Headers["X-Webhook-Signature"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(providedSig) ||
                !VerifyHmacSha256(body, _boxtal.V3WebhookSecret!, providedSig))
                return Unauthorized();

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            // ✅ Ta vraie clé de rapprochement
            var shippingOrderId = root.TryGetProperty("shippingOrderId", out var so)
                                && so.ValueKind == JsonValueKind.String ? so.GetString() : null;

            if (!root.TryGetProperty("payload", out var payload) ||
                !payload.TryGetProperty("trackings", out var trackings) ||
                trackings.ValueKind != JsonValueKind.Array)
                return Ok(); // rien à traiter

            int updated = 0;
            foreach (var t in trackings.EnumerateArray())
            {
                var trackingNumber = GetString(t, "trackingNumber");
                var trackingUrl = GetString(t, "packageTrackingUrl");
                var statusRaw = GetString(t, "status");

                // 🔎 Résolution commande : priorité shippingOrderId, puis n° de suivi
                var order = (await _orders.GetOrdersAsync()).FirstOrDefault(o => o.BoxtalShipmentId == shippingOrderId);

                if (order is null)
                {
                    _logger.LogWarning("Webhook Boxtal: commande introuvable (shippingOrderId={Ship}, tracking={Track})",
                        shippingOrderId, trackingNumber);
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(trackingNumber)) order.TrackingNumber = trackingNumber!;
                if (!string.IsNullOrWhiteSpace(statusRaw)) order.Status = TrackingStatus.GetTrackingStatus(statusRaw!);
                if (!string.IsNullOrWhiteSpace(trackingUrl)) order.TrackingLink = trackingUrl;


                await _orders.UpdateOrdersAsync(order);
                updated++;
            }

            _logger.LogInformation("Webhook Boxtal: {Count} commande(s) mise(s) à jour.", updated);
            return Ok();

            // -- helpers --
            static bool VerifyHmacSha256(string rawBody, string secret, string providedHeader)
            {
                var provided = providedHeader.Replace("sha256=", "", StringComparison.OrdinalIgnoreCase).Trim();
                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
                var expectedHex = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(rawBody))).ToLowerInvariant();
                return CryptographicOperations.FixedTimeEquals(
                    Encoding.ASCII.GetBytes(expectedHex),
                    Encoding.ASCII.GetBytes(provided.ToLowerInvariant()));
            }

            static string? GetString(JsonElement obj, string name)
                => obj.TryGetProperty(name, out var p) && p.ValueKind == JsonValueKind.String ? p.GetString() : null;
        }



        [HttpGet("/shipping/{id}/tracking")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<AspNetRoleViewModel>), Description = "list of roles")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "An unexpected error occurred")]
        public async Task<IActionResult> GetRolesAsync([FromRoute] string id)
        {
            var result = await _boxalProviderViewModelBuilder.GetShippingTrackingAsync(id);
            return Ok(result);
        }

        // ================== Utils ==================
        private static string GenerateHexSecret(int numBytes)
        {
            var bytes = RandomNumberGenerator.GetBytes(numBytes);
            return Convert.ToHexString(bytes).ToLowerInvariant(); // 2 chars/byte -> 64 chars pour 32 octets
        }
    }

    // petit helper d’extension pour JSON (legacy)
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
