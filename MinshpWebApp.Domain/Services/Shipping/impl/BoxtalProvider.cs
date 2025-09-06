using Microsoft.Extensions.Options;
using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using CreateShipmentCmdModel = MinshpWebApp.Domain.Models.CreateShipmentCmd;
using Package = MinshpWebApp.Domain.Models.Package;

namespace MinshpWebApp.Domain.Services.Shipping.impl
{
    /* ===================== Modèles d'aide (désérialisation souple) ===================== */


    public sealed class ParcelPointResponse
    {
        [JsonPropertyName("items")] public List<ParcelPointItem>? Items { get; set; }
        [JsonPropertyName("content")] public List<ParcelPointBucket>? Content { get; set; }
    }

    public sealed class ParcelPointBucket
    {
        [JsonPropertyName("parcelPoints")] public List<ParcelPointItem>? ParcelPoints { get; set; }
        [JsonPropertyName("parcelpoints")] public List<ParcelPointItem>? Parcelpoints { get; set; } // variation minuscule
    }

    public sealed class ParcelPointItem
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("code")] public string? Code { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("label")] public string? Label { get; set; }

        [JsonPropertyName("address")] public ParcelAddress? Address { get; set; }
        [JsonPropertyName("latitude")] public double? Latitude { get; set; }
        [JsonPropertyName("longitude")] public double? Longitude { get; set; }
        [JsonPropertyName("geo")] public Geo? Geo { get; set; } // { lat, lng }
    }

    public sealed class ParcelAddress
    {
        [JsonPropertyName("line1")] public string? Line1 { get; set; }
        [JsonPropertyName("address")] public string? Address { get; set; }

        // deux variantes possibles
        [JsonPropertyName("zipCode")] public string? ZipCode { get; set; }
        [JsonPropertyName("zipcode")] public string? Zipcode { get; set; }

        [JsonPropertyName("city")] public string? City { get; set; }
    }

    public sealed class Geo
    {
        [JsonPropertyName("lat")] public double? Lat { get; set; }
        [JsonPropertyName("lng")] public double? Lng { get; set; }
    }

    /* ===================== Options ===================== */

    public class BoxtalOptions
    {
        public string BaseUrlV3 { get; set; } = "https://api.boxtal.build";
        public string BaseUrlV1 { get; set; } = "https://test.envoimoinscher.com/api/v1";

        // v3 (app) - non utilisés ici car l’auth se fait via Basic portail
        public string AppId { get; set; } = "";
        public string AppSecret { get; set; } = "";

        // v1 (portail)
        public string V1UserName { get; set; } = "";
        public string V1Password { get; set; } = "";

        // expéditeur par défaut
        public string FromZip { get; set; } = "91000";
        public string FromCountry { get; set; } = "FR";
        public string FromCity { get; set; } = "Evry-courcouronnes";

        public string? FromEmail { get; set; } = "minamba.c@gmail.com";
        public string? FromPhone { get; set; } = "06 24 95 75 58";
        public string? FromCompany { get; set; } = "Mins Shop";
        public string? FromFirstName { get; set; } = "Minamba";
        public string? FromLastName { get; set; } = "Camara";
        public int? FromNumber { get; set; } = 2;
        public string? FromStreet { get; set; } = "rue des jules vallès";
        public string? FromAdditionalInfo { get; set; }
        public string? ReturnAdditionalInfo { get; set; } = "Retour Mins Shop";
    }

    /* ===================== Provider ===================== */

    public class BoxtalProvider : IShippingProvider
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly BoxtalOptions _opt;

        private string? _tokenV3;
        private DateTime _tokenExpUtc;

        public BoxtalProvider(IHttpClientFactory httpFactory, IOptions<BoxtalOptions> opt)
        {
            _httpFactory = httpFactory;
            _opt = opt.Value;
        }

        /* -------- Auth v3 -------- */
        private async Task<string> GetTokenV3Async()
        {
            if (!string.IsNullOrEmpty(_tokenV3) && DateTime.UtcNow < _tokenExpUtc.AddSeconds(-60))
                return _tokenV3!;

            var client = _httpFactory.CreateClient("BoxtalV3");
            var req = new HttpRequestMessage(HttpMethod.Post, "/iam/account-app/token");

            // Basic avec IDENTIFIANTS PORTAIL (email/mot de passe)
            var basic = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_opt.V1UserName}:{_opt.V1Password}"));
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);

            var resp = await client.SendAsync(req);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Auth v3 failed {(int)resp.StatusCode} {body}");

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            _tokenV3 = root.GetProperty("accessToken").GetString();
            var ttl = root.GetProperty("expiresIn").GetInt32();
            _tokenExpUtc = DateTime.UtcNow.AddSeconds(ttl);

            return _tokenV3!;
        }

        /* -------- RATES (v1 /cotation, XML) -------- */
        public async Task<List<Rate>> GetRatesAsync(OrderDetails orderDetails)
        {
            var client = _httpFactory.CreateClient("BoxtalV1");
            var basic = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_opt.V1UserName}:{_opt.V1Password}"));

            string codeHighTech = null;
            var packagesLst = orderDetails.Packages.ToList();

            bool b = packagesLst.Any(c => c.ContainedCode == "50110");

            if(b)
                codeHighTech = packagesLst?.FirstOrDefault(c => c.ContainedCode == "50110").ContainedCode ?? null;


            var qs = new Dictionary<string, string?>
            {
                ["expediteur.pays"] = _opt.FromCountry,
                ["expediteur.code_postal"] = _opt.FromZip,
                ["expediteur.ville"] = _opt.FromCity,
                ["expediteur.type"] = orderDetails.SenderType.ToLower(),

                ["destinataire.ville"] = orderDetails.RecipientCity,
                ["destinataire.pays"] = orderDetails.RecipientCountry,
                ["destinataire.code_postal"] = orderDetails.RecipientZipCode,
                ["destinataire.type"] = orderDetails.RecipientType.ToLower(),

                ["code_contenu"] = codeHighTech ?? packagesLst[0].ContainedCode,
            };


  

            // Puis les colis (1, 2, 3, ...)
            for (int i = 0; i < packagesLst.Count; i++)
            {
                var package = packagesLst[i];
                var n = i + 1;
                var prefix = $"colis_{n}";

                qs[$"{prefix}.poids"] = package.PackageWeight?.ToString(CultureInfo.InvariantCulture);
                qs[$"{prefix}.valeur"] = package.PackageValue?.ToString(CultureInfo.InvariantCulture);
                qs[$"{prefix}.largeur"] = package.PackageWidth;
                qs[$"{prefix}.hauteur"] = package.PackageHeight;
                qs[$"{prefix}.longueur"] = package.PackageLonger;
            }

            var query = string.Join("&", qs.Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                                           .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));

            var uri = new Uri(client.BaseAddress!, $"cotation?{query}");
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);

            var resp = await client.SendAsync(req);
            var text = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"/cotation failed {(int)resp.StatusCode} {resp.ReasonPhrase}\n{text}");

            var trimmed = text.TrimStart();
            if (trimmed.StartsWith("{") || trimmed.StartsWith("["))
                return ParseJsonRates(text);

            return ParseXmlRates(text);
        }

        private static List<Rate> ParseJsonRates(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var offers = root.ValueKind == JsonValueKind.Array ? root
                       : (root.TryGetProperty("offers", out var arr) ? arr : default);

            var list = new List<Rate>();
            if (offers.ValueKind == JsonValueKind.Array)
            {
                foreach (var o in offers.EnumerateArray())
                {
                    var code = (o.TryGetProperty("service", out var s) ? s.GetString()
                               : o.TryGetProperty("code", out var c) ? c.GetString()
                               : "UNKNOWN") ?? "UNKNOWN";

                    var label = o.TryGetProperty("label", out var l) ? (l.GetString() ?? code) : code;
                    var carrier = o.TryGetProperty("carrier", out var cr) ? (cr.GetString() ?? "Boxtal") : "Boxtal";

                    decimal price = 0m;
                    if (o.TryGetProperty("total", out var t) && t.TryGetProperty("price", out var p)) price = SafeDecimal(p.GetRawText());
                    else if (o.TryGetProperty("price", out var p2)) price = SafeDecimal(p2.GetRawText());

                    var isRelay = o.TryGetProperty("relay", out var r) && r.ValueKind == JsonValueKind.True;

                    list.Add(new Rate()
                    {
                        Code = code,
                        Carrier = carrier,
                        Label = label,
                        IsRelay = isRelay,
                        PriceTtc = price
                    });
                }
            }
            return list;
        }

        private static List<Rate> ParseXmlRates(string xml)
        {
            var doc = XDocument.Parse(xml);
            var offers = doc.Descendants().Where(x => x.Name.LocalName == "offer");
            var list = new List<Rate>();

            foreach (var o in offers)
            {
                string? productCode = o.ElementAny("code")?.Value;

                var service = o.ElementAny("service");
                string? svcCode = service?.ElementAny("code")?.Value;
                string? svcLabel = service?.ElementAny("label")?.Value;

                var op = o.ElementAny("operator");
                string? carrierNm = op?.ElementAny("label")?.Value ?? op?.ElementAny("name")?.Value;
                string? carrierCd = op?.ElementAny("code")?.Value;

                var priceNode = o.ElementAny("price");
                string? priceTtcT =
                       priceNode?.ElementAny("tax-inclusive")?.Value
                    ?? priceNode?.ElementAny("ttc")?.Value
                    ?? priceNode?.ElementAny("price")?.Value;
                decimal priceTtc = SafeDecimal(priceTtcT);

                string code = svcCode ?? productCode ?? "UNKNOWN";
                string label = svcLabel ?? code;
                string carrier = carrierNm ?? carrierCd ?? "Boxtal";
                bool isRelay = ContainsAny(label, "relais", "relay", "pickup", "shop2shop", "access point");

                list.Add(new Rate()
                {
                    Code = code,
                    Carrier = carrier,
                    Label = label,
                    IsRelay = isRelay,
                    PriceTtc = priceTtc
                });
            }
            return list;
        }


        public async Task<CodeCategories> GetContentCategoriesAsync()
        {
            var token = await GetTokenV3Async();
            var client = _httpFactory.CreateClient("BoxtalV3");

            var url = $"/shipping/v3.1/content-category?language={Uri.EscapeDataString("fr")}";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.SendAsync(req);
            var json = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"/parcel-point failed {(int)resp.StatusCode} {json}");

            // Parse selon: { content: [ { distanceFromSearchLocation, parcelPoint { ... } }, ... ] }
            using var doc = JsonDocument.Parse(json);
            var cdCategoriesLst = new CodeCategories();

            if (doc.RootElement.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in content.EnumerateArray())
                {
                        var cdCategory = new CodeCategory()
                        {
                            Id = item.Val("Id", "id") ?? "",
                            Label = item.Val("Label", "label") ?? ""
                        };
                        cdCategoriesLst.AllCodeCategories.Add(cdCategory);
                }
            }

            return cdCategoriesLst;
        }


        /* -------- RELAYS v3 -------- */

        // Recherche par CP/pays (structure simple), on tente de récupérer openingDays si présent
        public async Task<List<Relay>> GetRelaysAsync(string zip, string country, int limit = 20)
        {
            var token = await GetTokenV3Async();
            var client = _httpFactory.CreateClient("BoxtalV3");

            var url = $"/shipping/v3.1/parcel-point?zipCode={Uri.EscapeDataString(zip)}&countryCode={Uri.EscapeDataString(country)}&limit={limit}";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.SendAsync(req);
            var text = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"/parcel-point failed {(int)resp.StatusCode} {text}");

            using var doc = JsonDocument.Parse(text);
            var root = doc.RootElement;

            var items = root.ValueKind == JsonValueKind.Array ? root
                     : (root.TryGetProperty("items", out var arr) ? arr : default);

            var pts = new List<Relay>();
            if (items.ValueKind == JsonValueKind.Array)
            {
                foreach (var p in items.EnumerateArray())
                {
                    var opening = ParseOpeningDays(p);

                    pts.Add(new Relay(
                        id: p.Val("id", "code") ?? Guid.NewGuid().ToString("N"),
                        name: p.Val("name", "label") ?? "Point relais",
                        address: p.Val("address", "street") ?? "",
                        zip: p.Val("zipCode", "zip") ?? "",
                        city: p.Val("city") ?? "",
                        lat: p.TryGetProperty("latitude", out var la) ? la.GetDouble()
                                 : (p.TryGetProperty("geo", out var g) && g.TryGetProperty("lat", out var la2) ? la2.GetDouble() : 0),
                        lng: p.TryGetProperty("longitude", out var lo) ? lo.GetDouble()
                                 : (p.TryGetProperty("geo", out var g2) && g2.TryGetProperty("lng", out var lo2) ? lo2.GetDouble() : 0),
                        distance: p.TryGetProperty("distanceFromSearchLocation", out var di) && di.ValueKind == JsonValueKind.Number ? di.GetInt32() : 0,
                        network: p.Val("network") ?? "",
                        openingDays: opening
                    ));
                }
            }

            return pts;
        }

        // Recherche par adresse (distance présente au niveau de l'ITEM)
        public async Task<List<Relay>> GetRelaysByAddressAsync(RelaysAddress q)
        {
            var token = await GetTokenV3Async();
            var client = _httpFactory.CreateClient("BoxtalV3");

            // QS
            var qs = new List<string>();
            void Add(string k, string? v) { if (!string.IsNullOrWhiteSpace(v)) qs.Add($"{Uri.EscapeDataString(k)}={Uri.EscapeDataString(v)}"); }

            Add("number", q.Number);
            Add("street", q.Street);
            Add("city", q.City);
            Add("postalCode", q.PostalCode);
            Add("state", q.State);
            Add("countryIsoCode", q.CountryIsoCode);
            if (q.Limit > 0) Add("limit", q.Limit.ToString(CultureInfo.InvariantCulture));
            if (q.SearchNetworks != null)
                foreach (var n in q.SearchNetworks.Where(s => !string.IsNullOrWhiteSpace(s)))
                    Add("searchNetworks", n.Trim()); // clé répétée

            var url = "/shipping/v3.1/parcel-point?" + string.Join("&", qs);

            // Call
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var resp = await client.SendAsync(req);
            var json = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"/parcel-point failed {(int)resp.StatusCode} {json}");

            // Parse selon: { content: [ { distanceFromSearchLocation, parcelPoint { ... } }, ... ] }
            using var doc = JsonDocument.Parse(json);
            var list = new List<Relay>();

            if (doc.RootElement.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in content.EnumerateArray())
                {
                    if (!item.TryGetProperty("parcelPoint", out var p) || p.ValueKind != JsonValueKind.Object)
                        continue;

                    // distance AU NIVEAU ITEM
                    int distance = 0;
                    if (item.TryGetProperty("distanceFromSearchLocation", out var dEl) && dEl.ValueKind == JsonValueKind.Number)
                        distance = dEl.GetInt32();

                    // location + position
                    var loc = p.TryGetProperty("location", out var l) ? l : default;
                    var pos = (loc.ValueKind == JsonValueKind.Object && loc.TryGetProperty("position", out var ps)) ? ps : default;

                    double lat = 0, lng = 0;
                    if (pos.ValueKind == JsonValueKind.Object)
                    {
                        if (pos.TryGetProperty("latitude", out var la)) lat = la.GetDouble();
                        if (pos.TryGetProperty("longitude", out var lo)) lng = lo.GetDouble();
                    }
                    if (lat == 0 && p.TryGetProperty("latitude", out var la2)) lat = la2.GetDouble();
                    if (lng == 0 && p.TryGetProperty("longitude", out var lo2)) lng = lo2.GetDouble();

                    var opening = ParseOpeningDays(p);

                    list.Add(new Relay(
                        id: p.Val("id", "code") ?? Guid.NewGuid().ToString("N"),
                        name: p.Val("name", "label") ?? "Point relais",
                        address: loc.Val("street") ?? "",
                        zip: loc.Val("postalCode") ?? "",
                        city: loc.Val("city") ?? "",
                        lat: lat,
                        lng: lng,
                        distance: distance,                 // ✅ correct
                        network: p.Val("network") ?? "",
                        openingDays: opening
                    ));
                }
            }

            return list;
        }

        /* -------- CREATE SHIPMENT v3 -------- */
        public async Task<ShipmentResult> CreateShipmentAsync(CreateShipmentCmdModel cmd)
        {
            var token = await GetTokenV3Async();
            var client = _httpFactory.CreateClient("BoxtalV3");

            // ----- MAP Shipment (C# → schéma attendu Boxtal v3) -----
            var srcShipment = cmd.Shipment ?? new Shipment();
            var srcPackages = srcShipment.Packages;

            // Fallback sur le 1er colis pour dimensions/poids si besoin
            var p0 = srcPackages.FirstOrDefault();

            var fallbackWidth = cmd.PackageWidth ?? SafeInt(p0?.PackageWidth, 15);
            var fallbackHeight = cmd.PackageHeight ?? SafeInt(p0?.PackageHeight, 11);
            var fallbackLength = cmd.PackageLength ?? SafeInt(p0?.PackageLonger, 16);
            var fallbackWeight = cmd.WeightKg.HasValue ? (double)cmd.WeightKg.Value : SafeDouble(p0?.PackageWeight, 0.25);

            var mappedPackages = (srcPackages).Select(p => new
            {
                type = string.IsNullOrWhiteSpace(p.Type) ? "PARCEL" : p.Type,
                weight = SafeDouble(p.PackageWeight, fallbackWeight),
                width = SafeInt(p.PackageWidth, fallbackWidth),
                height = SafeInt(p.PackageHeight, fallbackHeight),
                length = SafeInt(p.PackageLonger, fallbackLength),
                value = (p.PackageValue.HasValue && p.PackageValue.Value > 0)
                            ? new { value = p.PackageValue.Value, currency = "EUR" }
                            : null,
                content = (string.IsNullOrWhiteSpace(cmd.ContentId) && string.IsNullOrWhiteSpace(cmd.ContentDescription))
                            ? null
                            : new { id = cmd.ContentId, description = cmd.ContentDescription },
                stackable = p.PackageStackable ?? true,
                externalId = !string.IsNullOrWhiteSpace(p.ExternalId)
                                ? p.ExternalId
                                : (!string.IsNullOrWhiteSpace(cmd.PackageExternalId) ? cmd.PackageExternalId : cmd.OrderExternalId)
            }).ToArray();

            // Types d’adresses en VAL v3
            string ToType(string? t) =>
                string.Equals(t, "BUSINESS", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(t, "entreprise", StringComparison.OrdinalIgnoreCase)
                    ? "BUSINESS" : "RESIDENTIAL";

            var mappedShipment = new
            {
                packages = mappedPackages,

                toAddress = new
                {
                    type = ToType(srcShipment.ToAddress?.Type),
                    contact = new
                    {
                        email = cmd.ToEmail ?? srcShipment.ToAddress?.Contact?.Email,
                        phone = cmd.ToPhone ?? srcShipment.ToAddress?.Contact?.Phone,
                        lastName = cmd.ToLastName ?? srcShipment.ToAddress?.Contact?.LastName,
                        firstName = cmd.ToFirstName ?? srcShipment.ToAddress?.Contact?.FirstName
                    },
                    location = new
                    {
                        city = cmd.ToCity ?? srcShipment.ToAddress?.Location?.City,
                        number = (int?)null, // si tu as un numéro précis sinon null
                        street = cmd.ToStreet ?? srcShipment.ToAddress?.Location?.Street,
                        postalCode = cmd.ToZip ?? srcShipment.ToAddress?.Location?.PostalCode,
                        countryIsoCode = string.IsNullOrWhiteSpace(cmd.ToCountry)
                                            ? (srcShipment.ToAddress?.Location?.CountryIsoCode ?? "FR")
                                            : cmd.ToCountry
                    }
                },

                externalId = cmd.OrderExternalId ?? srcShipment.ExternalId,

                fromAddress = new
                {
                    type = "BUSINESS",
                    contact = new
                    {
                        email = _opt.FromEmail,
                        phone = _opt.FromPhone,
                        company = _opt.FromCompany,
                        lastName = _opt.FromLastName,
                        firstName = _opt.FromFirstName
                    },
                    location = new
                    {
                        city = _opt.FromCity,
                        number = _opt.FromNumber,
                        street = _opt.FromStreet,
                        postalCode = _opt.FromZip,
                        countryIsoCode = _opt.FromCountry
                    },
                    additionalInformation = _opt.FromAdditionalInfo
                },

                returnAddress = new
                {
                    type = "BUSINESS",
                    contact = new
                    {
                        email = _opt.FromEmail,
                        phone = _opt.FromPhone,
                        company = _opt.FromCompany,
                        lastName = _opt.FromLastName,
                        firstName = _opt.FromFirstName
                    },
                    location = new
                    {
                        city = _opt.FromCity,
                        number = _opt.FromNumber,
                        street = _opt.FromStreet,
                        postalCode = _opt.FromZip,
                        countryIsoCode = _opt.FromCountry
                    },
                    additionalInformation = _opt.ReturnAdditionalInfo
                },

                pickupPointCode = (cmd.IsRelay == true) ? (cmd.RelayId ?? srcShipment.PickupPointCode) : null,
                dropOffPointCode = cmd.DropOffPointCode ?? srcShipment.DropOffPointCode
            };

            // ----- BODY RACINE (v3) -----
            var expectedDate = !string.IsNullOrWhiteSpace(cmd.ExpectedTakingOverDate)
                ? DateTime.Parse(cmd.ExpectedTakingOverDate, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")
                : DateTime.UtcNow.ToString("yyyy-MM-dd");

            var body = new
            {
                insured = cmd.Insured ?? false,
                labelType = string.IsNullOrWhiteSpace(cmd.LabelType) ? "PDF_A4" : cmd.LabelType,

                // v3 exige AU MOINS un des deux :
                shippingOfferCode = "CHRP", //string.IsNullOrWhiteSpace(cmd.ServiceCode) ? null : cmd.ServiceCode,
                shippingOfferId = "CHRP-736BX",//string.IsNullOrWhiteSpace(cmd.ShippingOfferId) ? null : cmd.ShippingOfferId,

                expectedTakingOverDate = expectedDate,

                shipment = mappedShipment
            };

            var jsonOpts = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var req = new HttpRequestMessage(HttpMethod.Post, "/shipping/v3.1/shipping-order");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            req.Content = new StringContent(JsonSerializer.Serialize(body, jsonOpts), Encoding.UTF8, "application/json");

            var resp = await client.SendAsync(req);
            var text = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"/shipping-order failed {(int)resp.StatusCode} {text}");

            using var doc = JsonDocument.Parse(text);
            var r = doc.RootElement;

            return new ShipmentResult(
                providerShipmentId: r.TryGetProperty("id", out var id) ? id.GetString()! : "",
                carrier: r.TryGetProperty("carrierCode", out var cc) ? (cc.GetString() ?? "Boxtal") : "Boxtal",
                serviceCode: r.Val("productCode", "serviceCode") ?? "",
                trackingNumber: r.TryGetProperty("trackingNumber", out var tr) ? tr.GetString() ?? "" : "",
                labelUrl: r.TryGetProperty("labelUrl", out var lu) ? lu.GetString() ?? "" : ""
            );
        }





        /* ===================== Helpers ===================== */

        private static Dictionary<string, List<OpeningInterval>>? ParseOpeningDays(JsonElement p)
        {
            if (!p.TryGetProperty("openingDays", out var od) || od.ValueKind != JsonValueKind.Object)
                return null;

            var result = new Dictionary<string, List<OpeningInterval>>(StringComparer.OrdinalIgnoreCase);

            foreach (var dayProp in od.EnumerateObject())
            {
                if (dayProp.Value.ValueKind != JsonValueKind.Array) continue;

                var list = new List<OpeningInterval>();
                foreach (var slot in dayProp.Value.EnumerateArray())
                {
                    var open = slot.TryGetProperty("openingTime", out var ot) ? (ot.GetString() ?? "") : "";
                    var close = slot.TryGetProperty("closingTime", out var ct) ? (ct.GetString() ?? "") : "";
                    if (!string.IsNullOrEmpty(open) || !string.IsNullOrEmpty(close))
                        list.Add(new OpeningInterval(open, close));
                }
                result[dayProp.Name] = list;
            }

            return result.Count == 0 ? null : result;
        }

        private static bool ContainsAny(string? s, params string[] needles)
            => !string.IsNullOrEmpty(s) && needles.Any(n => s.Contains(n, StringComparison.OrdinalIgnoreCase));

        private static decimal SafeDecimal(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0m;
            s = s.Trim().Replace(',', '.');
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0m;
        }



        private static int SafeInt(string? s, int fallback) =>
        int.TryParse((s ?? "").Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : fallback;

        private static double SafeDouble(string? s, double fallback) =>
            double.TryParse((s ?? "").Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : fallback;

    }

    internal static class JsonElementExt
    {
        public static string? Val(this JsonElement e, params string[] names)
        {
            foreach (var n in names)
            {
                if (e.TryGetProperty(n, out var v))
                {
                    if (v.ValueKind == JsonValueKind.String) return v.GetString();
                    if (v.ValueKind is JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False)
                        return v.GetRawText();
                }
            }
            return null;
        }
    }

    internal static class XElementExt
    {
        public static XElement? ElementAny(this XElement e, params string[] names)
        {
            foreach (var n in names)
            {
                var found = e.Element(e.GetDefaultNamespace() + n) ?? e.Element(n);
                if (found != null) return found;
            }
            return e.Elements().FirstOrDefault(x => names.Contains(x.Name.LocalName, StringComparer.OrdinalIgnoreCase));
        }

        public static string? Val(this XElement e, params string[] names)
            => ElementAny(e, names)?.Value;
    }


}
