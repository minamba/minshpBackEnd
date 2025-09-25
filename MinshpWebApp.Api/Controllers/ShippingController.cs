using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Request;
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
            // 0) Idempotence (optionnel)
            var eventId = Request.Headers["X-Event-Id"].FirstOrDefault()
                       ?? Request.Headers["X-Delivery-Id"].FirstOrDefault()
                       ?? Request.Headers["X-Request-Id"].FirstOrDefault();
            if (!string.IsNullOrEmpty(eventId))
            {
                var cacheKey = $"boxtal:v3:event:{eventId}";
                if (_cache.TryGetValue(cacheKey, out _)) return Ok();
                _cache.Set(cacheKey, true, TimeSpan.FromMinutes(10));
            }

            // 1) Lire le corps brut
            string body;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                body = await reader.ReadToEndAsync();

            // 2) Vérifier présence du secret + signature fournie
            if (string.IsNullOrEmpty(_boxtal.V3WebhookSecret))
                return StatusCode((int)HttpStatusCode.PreconditionRequired, "V3WebhookSecret non configuré.");

            var providedSig = GetSignatureHeader();                 // <-- une seule lecture
            if (string.IsNullOrWhiteSpace(providedSig))
            {
                _logger.LogWarning("Webhook: signature header manquant.");
                return Unauthorized(); // ou BadRequest()
            }

            // 3) Comparaison HMAC en temps constant
            if (!VerifyHmacSha256(body, _boxtal.V3WebhookSecret!, providedSig))
            {
                _logger.LogWarning("Webhook: signature invalide.");
                return Unauthorized();
            }

            // 4) Désérialiser et traiter
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var evt = JsonSerializer.Deserialize<V3TrackingEvent>(body, opts);
            if (evt?.Content is null || evt.Content.Count == 0) return Ok();

            foreach (var pkg in evt.Content)
            {
                var external = pkg.PackageExternalId ?? pkg.PackageId;
                if (string.IsNullOrWhiteSpace(external)) continue;

                var order = await _orders.GetByIdAsync(external);
                if (order is null) continue;

                if (!string.IsNullOrWhiteSpace(pkg.TrackingNumber)) order.TrackingNumber = pkg.TrackingNumber;
                if (!string.IsNullOrWhiteSpace(pkg.PackageTrackingUrl)) order.LabelUrl = pkg.PackageTrackingUrl;
                if (!string.IsNullOrWhiteSpace(pkg.Status)) order.Status = MapStatus(pkg.Status);

                await _orders.UpdateOrdersAsync(order);
            }

            return Ok();

            // ===== Helpers =====
            string? GetSignatureHeader()
            {
                var candidates = new[]
                {
            _boxtal.V3SignatureHeader ?? "X-Webhook-Signature",
            "X-Webhook-Signature",
            "X-Boxtal-Signature",
            "X-Hub-Signature-256",
            "X-Hub-Signature"
        };
                foreach (var h in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    var v = Request.Headers[h].FirstOrDefault();
                    if (!string.IsNullOrEmpty(v)) return v;
                }
                return null;
            }

            static bool VerifyHmacSha256(string rawBody, string secret, string? providedHeader)
            {
                if (string.IsNullOrEmpty(providedHeader)) return false;

                // Boxtal peut éventuellement préfixer "sha256=" ; on le retire si présent
                var provided = providedHeader.Replace("sha256=", "", StringComparison.OrdinalIgnoreCase).Trim();

                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
                var expected = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(rawBody))).ToLowerInvariant();

                return CryptographicOperations.FixedTimeEquals(
                    Encoding.ASCII.GetBytes(expected),
                    Encoding.ASCII.GetBytes(provided.ToLowerInvariant()));
            }

            static string MapStatus(string s) => s.Trim().ToUpperInvariant() switch
            {
                "ANNOUNCED" => "Préparée",
                "IN_TRANSIT" => "Expédiée",
                "DELIVERED" => "Livrée",
                "CANCELLED" => "Annulée",
                _ => s
            };
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
