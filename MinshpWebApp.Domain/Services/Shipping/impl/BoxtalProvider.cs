using Microsoft.Extensions.Options;
using MinshpWebApp.Domain.Enums;
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
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
//using CreateShipmentCmdModel = MinshpWebApp.Domain.Models.CreateShipmentCmd; //pour la v3
using CreateShipmentV1Cmd = MinshpWebApp.Domain.Models.CreateShipmentV1Cmd;

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
        public string V1PushUrlBase { get; set; } = "";
        public string V1PushToken { get; set; } = "";

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
        private readonly IApplicationService _applicationService;

        private string? _tokenV3;
        private DateTime _tokenExpUtc;

        public BoxtalProvider(IHttpClientFactory httpFactory, IOptions<BoxtalOptions> opt, IApplicationService applicationService)
        {
            _httpFactory = httpFactory;
            _opt = opt.Value;
            _applicationService = applicationService;
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

                // ---- Récupération des points relais (mandatory_informations)
                var dropOffCodes = ReadXmlEnumValuesForParam(o, "depot.pointrelais");
                var pickupCodes = ReadXmlEnumValuesForParam(o, "retrait.pointrelais");

                string code = svcCode ?? productCode ?? "UNKNOWN";
                string label = svcLabel ?? code;
                string carrier = carrierNm ?? carrierCd ?? "Boxtal";


                var delivery = o.ElementAny("delivery");
                var deliveryType = delivery?.ElementAny("type");
                var deliveryCode = deliveryType?.ElementAny("code")?.Value;



                bool isRelay = deliveryCode.Contains("PICKUP_POINT");

                List<string> allowCarrier = new List<string>
            {
                 "CHRONOPOST", "UPS", "MONDIAL RELAY", "LA POSTE"
            };


                if (allowCarrier.Contains(carrier.ToUpper()))
                {
                    list.Add(new Rate()
                    {
                        Code = code,
                        Carrier = carrier,
                        Label = label,
                        IsRelay = isRelay,
                        PriceTtc = priceTtc,
                        DropOffPointCodes = dropOffCodes,
                        PickupPointCodes = pickupCodes
                    });
                }
            }
            return list;

            // --- helpers locaux
            static List<string>? ReadXmlEnumValuesForParam(XElement offer, string wantedCode)
            {
                var mandatory = offer.ElementAny("mandatory_informations");
                if (mandatory == null) return null;

                foreach (var param in mandatory.Elements().Where(e => e.Name.LocalName == "parameter"))
                {
                    var code = param.ElementAny("code")?.Value?.Trim();
                    if (!string.Equals(code, wantedCode, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // <parameter><type><enum><value>XYZ</value>...</enum></type></parameter>
                    var enumNode = param.Descendants().FirstOrDefault(e => e.Name.LocalName == "enum");
                    if (enumNode == null) return null;

                    var values = enumNode.Elements().Where(e => e.Name.LocalName == "value")
                                      .Select(v => (v.Value ?? "").Trim())
                                      .Where(v => !string.IsNullOrWhiteSpace(v))
                                      .Distinct(StringComparer.OrdinalIgnoreCase)
                                      .ToList();

                    return values.Count == 0 ? null : values;
                }
                return null;
            }
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


        // RECHERCHE RELAYS PAR ADRESSE POUR LA V3
        //public async Task<List<Relay>> GetRelaysByAddressAsync(RelaysAddress q)
        //{
        //    var token = await GetTokenV3Async();
        //    var client = _httpFactory.CreateClient("BoxtalV3");

        //    // QS
        //    var qs = new List<string>();
        //    void Add(string k, string? v) { if (!string.IsNullOrWhiteSpace(v)) qs.Add($"{Uri.EscapeDataString(k)}={Uri.EscapeDataString(v)}"); }

        //    Add("number", q.Number);
        //    Add("street", q.Street);
        //    Add("city", q.City);
        //    Add("postalCode", q.PostalCode);
        //    Add("state", q.State);
        //    Add("countryIsoCode", q.CountryIsoCode);
        //    if (q.Limit > 0) Add("limit", q.Limit.ToString(CultureInfo.InvariantCulture));
        //    if (q.SearchNetworks != null)
        //        foreach (var n in q.SearchNetworks.Where(s => !string.IsNullOrWhiteSpace(s)))
        //            Add("searchNetworks", n.Trim()); // clé répétée

        //    var url = "/shipping/v3.1/parcel-point?" + string.Join("&", qs);

        //    // Call
        //    var req = new HttpRequestMessage(HttpMethod.Get, url);
        //    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var resp = await client.SendAsync(req);
        //    var json = await resp.Content.ReadAsStringAsync();
        //    if (!resp.IsSuccessStatusCode)
        //        throw new InvalidOperationException($"/parcel-point failed {(int)resp.StatusCode} {json}");

        //    // Parse selon: { content: [ { distanceFromSearchLocation, parcelPoint { ... } }, ... ] }
        //    using var doc = JsonDocument.Parse(json);
        //    var list = new List<Relay>();

        //    if (doc.RootElement.TryGetProperty("content", out var content) && content.ValueKind == JsonValueKind.Array)
        //    {
        //        foreach (var item in content.EnumerateArray())
        //        {
        //            if (!item.TryGetProperty("parcelPoint", out var p) || p.ValueKind != JsonValueKind.Object)
        //                continue;

        //            // distance AU NIVEAU ITEM
        //            int distance = 0;
        //            if (item.TryGetProperty("distanceFromSearchLocation", out var dEl) && dEl.ValueKind == JsonValueKind.Number)
        //                distance = dEl.GetInt32();

        //            // location + position
        //            var loc = p.TryGetProperty("location", out var l) ? l : default;
        //            var pos = (loc.ValueKind == JsonValueKind.Object && loc.TryGetProperty("position", out var ps)) ? ps : default;

        //            double lat = 0, lng = 0;
        //            if (pos.ValueKind == JsonValueKind.Object)
        //            {
        //                if (pos.TryGetProperty("latitude", out var la)) lat = la.GetDouble();
        //                if (pos.TryGetProperty("longitude", out var lo)) lng = lo.GetDouble();
        //            }
        //            if (lat == 0 && p.TryGetProperty("latitude", out var la2)) lat = la2.GetDouble();
        //            if (lng == 0 && p.TryGetProperty("longitude", out var lo2)) lng = lo2.GetDouble();

        //            var opening = ParseOpeningDays(p);

        //            list.Add(new Relay(
        //                id: p.Val("id", "code") ?? Guid.NewGuid().ToString("N"),
        //                name: p.Val("name", "label") ?? "Point relais",
        //                address: loc.Val("street") ?? "",
        //                zip: loc.Val("postalCode") ?? "",
        //                city: loc.Val("city") ?? "",
        //                lat: lat,
        //                lng: lng,
        //                distance: distance,                 // ✅ correct
        //                network: p.Val("network") ?? "",
        //                openingDays: opening
        //            ));
        //        }
        //    }

        //    return list;
        //}



        // RECHERCHE RELAYS PAR ADRESSE POUR LA V1 (XML)
        public async Task<List<Relay>> GetRelaysByAddressAsync(RelaysAddress q)
        {
            var client = _httpFactory.CreateClient("BoxtalV1");
            var basic = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_opt.V1UserName}:{_opt.V1Password}"));

            // Adresse "numéro + rue"
            string? fullAddress = null;
            if (!string.IsNullOrWhiteSpace(q.Number) || !string.IsNullOrWhiteSpace(q.Street))
                fullAddress = $"{q.Number ?? ""} {q.Street ?? ""}".Trim();

            // ---- QS V1 /listpoints ----
            // collecte = "dest" (points pour le destinataire) ou "exp" (points pour l'expéditeur).
            var qs = new List<string>();
            void Add(string k, string? v) { if (!string.IsNullOrWhiteSpace(v)) qs.Add($"{Uri.EscapeDataString(k)}={Uri.EscapeDataString(v)}"); }

            Add("collecte", "dest"); // mets "exp" si tu cherches un point de dépôt
            Add("pays", string.IsNullOrWhiteSpace(q.CountryIsoCode) ? "FR" : q.CountryIsoCode);
            Add("ville", q.City);
            Add("cp", q.PostalCode);
            Add("adresse", fullAddress);

            var network = q.SearchNetworks.ToList();
            string[] namesOfNewtowrk = Enum.GetNames<CarrierEnum>();


            // carriers[N]=MONR|CHRP|COPR|SOGP|UPSE
            if (network.Count() == 0)
            {
                network.AddRange(namesOfNewtowrk);
                q.SearchNetworks = network;
            }

            if (q.SearchNetworks != null)
            {
                int i = 0;
                foreach (var raw in q.SearchNetworks.Where(s => !string.IsNullOrWhiteSpace(s)))
                {
                    var code = raw.Trim().ToUpperInvariant();
                    qs.Add($"{Uri.EscapeDataString($"carriers[{i}]")}={Uri.EscapeDataString(code)}");
                    i++;
                }
            }

            var url = "listpoints" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");

            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);
            req.Headers.Accept.ParseAdd("application/xml");

            var resp = await client.SendAsync(req);
            var text = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"/listpoints failed {(int)resp.StatusCode} {resp.ReasonPhrase}\n{text}");

            // ---- Parse XML ----
            var relays = new List<Relay>();
            var x = XDocument.Parse(text);

            // <carriers><carrier><operator>MONR</operator><points><point>...</point></points></carrier>...
            var carriers = x.Descendants().Where(e => e.Name.LocalName.Equals("carrier", StringComparison.OrdinalIgnoreCase));
            foreach (var carrier in carriers)
            {
                var operatorCode = carrier.ElementAny("operator")?.Value ?? ""; // ex: MONR / CHRP / ...
                var pointsNode = carrier.ElementAny("points");
                if (pointsNode == null) continue;

                foreach (var p in pointsNode.Elements().Where(e => e.Name.LocalName.Equals("point", StringComparison.OrdinalIgnoreCase)))
                {
                    string id = p.ElementAny("code", "id")?.Value ?? Guid.NewGuid().ToString("N");
                    string name = p.ElementAny("name", "label", "nom")?.Value ?? "Point relais";
                    string addr = p.ElementAny("address", "adresse", "line1", "street")?.Value ?? "";
                    string zip = p.ElementAny("zipcode", "zipCode", "code_postal")?.Value ?? "";
                    string city = p.ElementAny("city", "ville")?.Value ?? "";

                    double lat = 0, lng = 0;
                    double.TryParse((p.ElementAny("latitude", "lat")?.Value ?? "").Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lat);
                    double.TryParse((p.ElementAny("longitude", "lng")?.Value ?? "").Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out lng);

                    var opening = ParseOpeningDays_ListPoints(p); // horaires <schedule><day>...

                    relays.Add(new Relay(
                        id: id,
                        name: name,
                        address: addr,
                        zip: zip,
                        city: city,
                        lat: lat,
                        lng: lng,
                        distance: 0,                 // /listpoints ne renvoie pas la distance
                        network: operatorCode,       // ex: MONR, CHRP, ...
                        openingDays: opening
                    ));
                }
            }

            return relays;
        }

        // ---- helper horaires pour /listpoints (weekday + open_am/pm, close_am/pm) ----
        private static Dictionary<string, List<OpeningInterval>>? ParseOpeningDays_ListPoints(XElement point)
        {
            var schedule = point.ElementAny("schedule");
            if (schedule == null) return null;

            var result = new Dictionary<string, List<OpeningInterval>>(StringComparer.OrdinalIgnoreCase);

            string DayKey(int weekday) => weekday switch
            {
                1 => "monday",
                2 => "tuesday",
                3 => "wednesday",
                4 => "thursday",
                5 => "friday",
                6 => "saturday",
                7 => "sunday",
                _ => "monday"
            };

            foreach (var day in schedule.Elements().Where(e => e.Name.LocalName.Equals("day", StringComparison.OrdinalIgnoreCase)))
            {
                var slots = new List<OpeningInterval>();

                var wText = day.ElementAny("weekday")?.Value;
                int.TryParse(wText, out var w);
                var key = DayKey(w);

                var openAm = day.ElementAny("open_am", "openingTimeAm", "openMorning")?.Value;
                var closeAm = day.ElementAny("close_am", "closingTimeAm", "closeMorning")?.Value;
                var openPm = day.ElementAny("open_pm", "openingTimePm", "openAfternoon")?.Value;
                var closePm = day.ElementAny("close_pm", "closingTimePm", "closeAfternoon")?.Value;

                if (!string.IsNullOrWhiteSpace(openAm) || !string.IsNullOrWhiteSpace(closeAm))
                    slots.Add(new OpeningInterval(openAm ?? "", closeAm ?? ""));
                if (!string.IsNullOrWhiteSpace(openPm) || !string.IsNullOrWhiteSpace(closePm))
                    slots.Add(new OpeningInterval(openPm ?? "", closePm ?? ""));

                if (slots.Count > 0) result[key] = slots;
            }

            return result.Count == 0 ? null : result;
        }



        /* -------- CREATE SHIPMENT v3 -------- */
        //public async Task<ShipmentResult> CreateShipmentAsync(CreateShipmentCmdModel cmd)
        //{
        //    var token = await GetTokenV3Async();
        //    var client = _httpFactory.CreateClient("BoxtalV3");

        //    // ----- MAP Shipment (C# → schéma attendu Boxtal v3) -----
        //    var srcShipment = cmd.Shipment ?? new Shipment();
        //    var srcPackages = srcShipment.Packages;

        //    // Fallback sur le 1er colis pour dimensions/poids si besoin
        //    var p0 = srcPackages.FirstOrDefault();

        //    var fallbackWidth = cmd.PackageWidth ?? SafeInt(p0?.PackageWidth, 15);
        //    var fallbackHeight = cmd.PackageHeight ?? SafeInt(p0?.PackageHeight, 11);
        //    var fallbackLength = cmd.PackageLength ?? SafeInt(p0?.PackageLonger, 16);
        //    var fallbackWeight = cmd.WeightKg.HasValue ? (double)cmd.WeightKg.Value : SafeDouble(p0?.PackageWeight, 0.25);

        //    var mappedPackages = (srcPackages).Select(p => new
        //    {
        //        type = string.IsNullOrWhiteSpace(p.Type) ? "PARCEL" : p.Type,
        //        weight = SafeDouble(p.PackageWeight, fallbackWeight),
        //        width = SafeInt(p.PackageWidth, fallbackWidth),
        //        height = SafeInt(p.PackageHeight, fallbackHeight),
        //        length = SafeInt(p.PackageLonger, fallbackLength),
        //        value = (p.PackageValue.HasValue && p.PackageValue.Value > 0)
        //                    ? new { value = p.PackageValue.Value, currency = "EUR" }
        //                    : null,
        //        content = (string.IsNullOrWhiteSpace(cmd.ContentId) && string.IsNullOrWhiteSpace(cmd.ContentDescription))
        //                    ? null
        //                    : new { id = cmd.ContentId, description = cmd.ContentDescription },
        //        stackable = p.PackageStackable ?? true,
        //        externalId = !string.IsNullOrWhiteSpace(p.ExternalId)
        //                        ? p.ExternalId
        //                        : (!string.IsNullOrWhiteSpace(cmd.PackageExternalId) ? cmd.PackageExternalId : cmd.OrderExternalId)
        //    }).ToArray();

        //    // Types d’adresses en VAL v3
        //    string ToType(string? t) =>
        //        string.Equals(t, "BUSINESS", StringComparison.OrdinalIgnoreCase) ||
        //        string.Equals(t, "entreprise", StringComparison.OrdinalIgnoreCase)
        //            ? "BUSINESS" : "RESIDENTIAL";

        //    var mappedShipment = new
        //    {
        //        packages = mappedPackages,

        //        toAddress = new
        //        {
        //            type = ToType(srcShipment.ToAddress?.Type),
        //            contact = new
        //            {
        //                email = cmd.ToEmail ?? srcShipment.ToAddress?.Contact?.Email,
        //                phone = cmd.ToPhone ?? srcShipment.ToAddress?.Contact?.Phone,
        //                lastName = cmd.ToLastName ?? srcShipment.ToAddress?.Contact?.LastName,
        //                firstName = cmd.ToFirstName ?? srcShipment.ToAddress?.Contact?.FirstName
        //            },
        //            location = new
        //            {
        //                city = cmd.ToCity ?? srcShipment.ToAddress?.Location?.City,
        //                number = (int?)null, // si tu as un numéro précis sinon null
        //                street = cmd.ToStreet ?? srcShipment.ToAddress?.Location?.Street,
        //                postalCode = cmd.ToZip ?? srcShipment.ToAddress?.Location?.PostalCode,
        //                countryIsoCode = string.IsNullOrWhiteSpace(cmd.ToCountry)
        //                                    ? (srcShipment.ToAddress?.Location?.CountryIsoCode ?? "FR")
        //                                    : cmd.ToCountry
        //            }
        //        },

        //        externalId = cmd.OrderExternalId ?? srcShipment.ExternalId,

        //        fromAddress = new
        //        {
        //            type = "BUSINESS",
        //            contact = new
        //            {
        //                email = _opt.FromEmail,
        //                phone = _opt.FromPhone,
        //                company = _opt.FromCompany,
        //                lastName = _opt.FromLastName,
        //                firstName = _opt.FromFirstName
        //            },
        //            location = new
        //            {
        //                city = _opt.FromCity,
        //                number = _opt.FromNumber,
        //                street = _opt.FromStreet,
        //                postalCode = _opt.FromZip,
        //                countryIsoCode = _opt.FromCountry
        //            },
        //            additionalInformation = _opt.FromAdditionalInfo
        //        },

        //        returnAddress = new
        //        {
        //            type = "BUSINESS",
        //            contact = new
        //            {
        //                email = _opt.FromEmail,
        //                phone = _opt.FromPhone,
        //                company = _opt.FromCompany,
        //                lastName = _opt.FromLastName,
        //                firstName = _opt.FromFirstName
        //            },
        //            location = new
        //            {
        //                city = _opt.FromCity,
        //                number = _opt.FromNumber,
        //                street = _opt.FromStreet,
        //                postalCode = _opt.FromZip,
        //                countryIsoCode = _opt.FromCountry
        //            },
        //            additionalInformation = _opt.ReturnAdditionalInfo
        //        },

        //        pickupPointCode = (cmd.IsRelay == true) ? (cmd.RelayId ?? srcShipment.PickupPointCode) : null,
        //        dropOffPointCode = cmd.DropOffPointCode ?? srcShipment.DropOffPointCode
        //    };

        //    // ----- BODY RACINE (v3) -----
        //    var expectedDate = !string.IsNullOrWhiteSpace(cmd.ExpectedTakingOverDate)
        //        ? DateTime.Parse(cmd.ExpectedTakingOverDate, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")
        //        : DateTime.UtcNow.ToString("yyyy-MM-dd");

        //    var body = new
        //    {
        //        insured = cmd.Insured ?? false,
        //        labelType = string.IsNullOrWhiteSpace(cmd.LabelType) ? "PDF_A4" : cmd.LabelType,

        //        // v3 exige AU MOINS un des deux :
        //        shippingOfferCode = "CHRP-Chrono2ShopDirect", //string.IsNullOrWhiteSpace(cmd.ServiceCode) ? null : cmd.ServiceCode,
        //        //shippingOfferId = "CHRP-736BX",//string.IsNullOrWhiteSpace(cmd.ShippingOfferId) ? null : cmd.ShippingOfferId,

        //        expectedTakingOverDate = expectedDate,

        //        shipment = mappedShipment
        //    };

        //    var jsonOpts = new JsonSerializerOptions
        //    {
        //        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        //    };

        //    var req = new HttpRequestMessage(HttpMethod.Post, "/shipping/v3.1/shipping-order");
        //    req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    req.Content = new StringContent(JsonSerializer.Serialize(body, jsonOpts), Encoding.UTF8, "application/json");

        //    var resp = await client.SendAsync(req);
        //    var text = await resp.Content.ReadAsStringAsync();

        //    if (!resp.IsSuccessStatusCode)
        //        throw new InvalidOperationException($"/shipping-order failed {(int)resp.StatusCode} {text}");

        //    using var doc = JsonDocument.Parse(text);
        //    var r = doc.RootElement;

        //    return new ShipmentResult(
        //        providerShipmentId: r.TryGetProperty("id", out var id) ? id.GetString()! : "",
        //        carrier: r.TryGetProperty("carrierCode", out var cc) ? (cc.GetString() ?? "Boxtal") : "Boxtal",
        //        serviceCode: r.Val("productCode", "serviceCode") ?? "",
        //        trackingNumber: r.TryGetProperty("trackingNumber", out var tr) ? tr.GetString() ?? "" : "",
        //        labelUrl: r.TryGetProperty("labelUrl", out var lu) ? lu.GetString() ?? "" : ""
        //    );
        //}





        /* -------- CREATE SHIPMENT v1 -------- */
        public async Task<ShipmentResult> CreateShipmentAsync(CreateShipmentV1Cmd cmd)
        {
            var client = _httpFactory.CreateClient("BoxtalV1");
            var basic = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_opt.V1UserName}:{_opt.V1Password}"));

            var form = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);


            // --- Expéditeur ---
            var fromCountry = string.IsNullOrWhiteSpace(cmd.FromCountry) ? _opt.FromCountry : cmd.FromCountry;
            var fromPhone = string.IsNullOrWhiteSpace(cmd.FromPhone) ? _opt.FromPhone : cmd.FromPhone;
            form["expediteur.tel"] = NormalizeBoxtalPhone(fromPhone, fromCountry);


            // --- Destinataire ---
            var toCountry = string.IsNullOrWhiteSpace(cmd.ToCountry) ? "FR" : cmd.ToCountry;
            var toPhone = string.IsNullOrWhiteSpace(cmd.ToPhone) ? "" : cmd.ToPhone;
            var ToPhone = NormalizeBoxtalPhone(toPhone, toCountry);


            // --- url_push (requis) ---
            if (!string.IsNullOrWhiteSpace(_opt.V1PushUrlBase) && !string.IsNullOrWhiteSpace(_opt.V1PushToken))
            {
                var qpOrder = Uri.EscapeDataString(cmd.ExternalOrderId ?? "");
                var qpToken = Uri.EscapeDataString(_opt.V1PushToken ?? "");
                var sep = _opt.V1PushUrlBase.Contains('?') ? "&" : "?";
                form["url_push"] = $"{_opt.V1PushUrlBase}{sep}orderId={qpOrder}&token={qpToken}";
            }

            // --- Offre (requis) ---
            form["operator"] = cmd.OperatorCode;     // ex: CHRP
            form["service"] = cmd.ServiceCode;      // ex: Chrono2ShopDirect

            var application = (await _applicationService.GetApplicationAsync()).ToList();
            string defaultDropOffMondialRelay = application[0].DefaultDropOffMondialRelay ?? "";
            string defaultDropOffUps = application[0].DefaultDropOffUps ?? "";
            string defaultDropOffChronoPost = application[0].DefaultDropOffChronoPost ?? "";
            string defaultDropOffLaposte = application[0].DefaultDropLaposte ?? "";

            CarrierEnum getcarrierFromRequest = Enum.Parse<CarrierEnum>(cmd.OperatorCode.ToUpper());

            string defaultDropOff = getcarrierFromRequest switch
            {
                CarrierEnum.MONR => defaultDropOffMondialRelay,
                CarrierEnum.UPSE => defaultDropOffUps,
                CarrierEnum.CHRP => defaultDropOffChronoPost,
                CarrierEnum.POFR => defaultDropOffLaposte
            };

     
            if (!string.IsNullOrWhiteSpace(cmd.DropOffPointCode)) form["depot.pointrelais"] = cmd.IsRelay ? defaultDropOff : cmd.DropOffPointCode;
            if (!string.IsNullOrWhiteSpace(cmd.PickupPointCode)) form["retrait.pointrelais"] = cmd.PickupPointCode;

            // --- Expéditeur (plusieurs requis) ---
            form["expediteur.type"] = (cmd.FromType ?? "entreprise").ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(cmd.FromCivility)) form["expediteur.civilite"] = cmd.FromCivility;
            form["expediteur.nom"] = string.IsNullOrWhiteSpace(cmd.FromLastName) ? _opt.FromLastName : cmd.FromLastName;
            form["expediteur.prenom"] = string.IsNullOrWhiteSpace(cmd.FromFirstName) ? _opt.FromFirstName : cmd.FromFirstName;
            form["expediteur.societe"] = string.IsNullOrWhiteSpace(cmd.FromCompany) ? _opt.FromCompany : cmd.FromCompany;
            form["expediteur.email"] = string.IsNullOrWhiteSpace(cmd.FromEmail) ? _opt.FromEmail : cmd.FromEmail;
            form["expediteur.tel"] = NormalizeBoxtalPhone(fromPhone, fromCountry);
            form["expediteur.adresse"] = string.IsNullOrWhiteSpace(cmd.FromAddress) ? _opt.FromStreet : cmd.FromAddress;
            form["expediteur.code_postal"] = string.IsNullOrWhiteSpace(cmd.FromZip) ? _opt.FromZip : cmd.FromZip;
            form["expediteur.ville"] = string.IsNullOrWhiteSpace(cmd.FromCity) ? _opt.FromCity : cmd.FromCity;
            form["expediteur.pays"] = string.IsNullOrWhiteSpace(cmd.FromCountry) ? _opt.FromCountry : cmd.FromCountry;

            // --- Destinataire (plusieurs requis) ---
            form["destinataire.type"] = (cmd.ToType ?? "particulier").ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(cmd.ToCivility)) form["destinataire.civilite"] = cmd.ToCivility;
            if (!string.IsNullOrWhiteSpace(cmd.ToLastName)) form["destinataire.nom"] = cmd.ToLastName;
            if (!string.IsNullOrWhiteSpace(cmd.ToFirstName)) form["destinataire.prenom"] = cmd.ToFirstName;
            if (!string.IsNullOrWhiteSpace(cmd.ToEmail)) form["destinataire.email"] = cmd.ToEmail;
            form["destinataire.tel"] = NormalizeBoxtalPhone(toPhone, toCountry);
            if (!string.IsNullOrWhiteSpace(cmd.ToAddress)) form["destinataire.adresse"] = cmd.ToAddress;
            if (!string.IsNullOrWhiteSpace(cmd.ToZip)) form["destinataire.code_postal"] = cmd.ToZip;
            if (!string.IsNullOrWhiteSpace(cmd.ToCity)) form["destinataire.ville"] = cmd.ToCity;
            if (!string.IsNullOrWhiteSpace(cmd.ToCountry)) form["destinataire.pays"] = cmd.ToCountry;

            // --- Contenu colis (requis) ---
            // code_contenu (requis) -> DOIT venir du front ou d’un mapping interne
            if (cmd.ContentCode != null)
                form["code_contenu"] = cmd.ContentCode.ToString(CultureInfo.InvariantCulture);
            else
                form["code_contenu"] = "50110"; // ⚠️ fallback de test : remplace par un vrai code

            // description (requis)
            form["colis.description"] = string.IsNullOrWhiteSpace(cmd.ContentDescription)
                ? $"Commande {cmd.ExternalOrderId}"
                : cmd.ContentDescription;


            //if (cmd.Packages?.Count == 1)
                //form["colis.valeur"] = cmd.DeclaredValue.Value.ToString(CultureInfo.InvariantCulture);


            // --- Colis N ---
            var pkgs = cmd.Packages?.Count > 0 ? cmd.Packages : new List<Package>();
            for (int i = 0; i < pkgs.Count; i++)
            {
                var p = pkgs[i]; var n = i + 1; var prefix = $"colis_{n}";
                if (!string.IsNullOrWhiteSpace(p.PackageWeight)) form[$"{prefix}.poids"] = p.PackageWeight!.Replace(',', '.');
                if (!string.IsNullOrWhiteSpace(p.PackageLonger)) form[$"{prefix}.longueur"] = p.PackageLonger;
                if (!string.IsNullOrWhiteSpace(p.PackageWidth)) form[$"{prefix}.largeur"] = p.PackageWidth;
                if (!string.IsNullOrWhiteSpace(p.PackageHeight)) form[$"{prefix}.hauteur"] = p.PackageHeight;
                if (p.PackageValue.HasValue) form[$"{prefix}.valeur"] = p.PackageValue.Value.ToString(CultureInfo.InvariantCulture);
            }
            // Fallback minimal si aucun colis renseigné (évite 400)
            if (!form.Keys.Any(k => k.StartsWith("colis_", StringComparison.Ordinal)))
            {
                form["colis_1.poids"] = "0.25";
                form["colis_1.longueur"] = "20";
                form["colis_1.largeur"] = "15";
                form["colis_1.hauteur"] = "10";
            }

            // --- Collecte (requis) ---
            var collect = /*cmd.CollectDate ?? cmd.TakeOverDate ??*/ DateTime.UtcNow.Date;
            form["collecte"] = collect.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            // --- Référence externe (nom V1) ---
            if (!string.IsNullOrWhiteSpace(cmd.ExternalOrderId))
                form["reference_externe"] = cmd.ExternalOrderId; // <-- remplace "external.id"

            // (facultatif) assurance / raison export
            //if (cmd.InsuranceSelection.HasValue) form["assurance.selection"] = cmd.InsuranceSelection.Value ? "true" : "false";
            //if (!string.IsNullOrWhiteSpace(cmd.Reason)) form["raison"] = cmd.Reason;

            // --- POST /order ---
            var payload = form.Where(kv => !string.IsNullOrWhiteSpace(kv.Value))
                              .ToDictionary(k => k.Key, v => v.Value!);

            // (Debug utile) : log des paires envoyées
            // foreach (var kv in payload) Console.WriteLine($"{kv.Key} = {kv.Value}");

            var req = new HttpRequestMessage(HttpMethod.Post, "order")
            { Content = new FormUrlEncodedContent(payload) };
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);
            req.Headers.Accept.ParseAdd("application/xml");

            var resp = await client.SendAsync(req);
            var text = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"/order failed {(int)resp.StatusCode} {resp.ReasonPhrase}\n{text}");

            try
            {
                var x = XDocument.Parse(text);
                string? shipmentId = x.Descendants().FirstOrDefault(e => e.Name.LocalName is "id" or "shipment_id" or "expedition_id" or "reference")?.Value;
                string? tracking = x.Descendants().FirstOrDefault(e => e.Name.LocalName is "tracking" or "tracking_number" or "numero_suivi")?.Value;
                string? labelUrl = x.Descendants().FirstOrDefault(e => e.Name.LocalName is "labelUrl" or "etiquette_url" or "url_etiquette")?.Value;

                return new ShipmentResult(
                    providerShipmentId: shipmentId ?? "",
                    carrier: cmd.OperatorCode,
                    serviceCode: cmd.ServiceCode,
                    trackingNumber: tracking ?? "",
                    labelUrl: labelUrl ?? ""
                );
            }
            catch
            {
                using var doc = JsonDocument.Parse(text);
                var r = doc.RootElement;
                return new ShipmentResult(
                    providerShipmentId: r.Val("id", "shipmentId") ?? "",
                    carrier: r.Val("carrier", "operator", "carrierCode") ?? "Boxtal",
                    serviceCode: cmd.ServiceCode,
                    trackingNumber: r.Val("trackingNumber", "tracking") ?? "",
                    labelUrl: r.Val("labelUrl", "label") ?? ""
                );
            }
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




        private static string NormalizeBoxtalPhone(string? phone, string? countryIso = "FR")
        {
            if (string.IsNullOrWhiteSpace(phone)) return "";

            // on ne garde que chiffres (et on détecte s'il y avait déjà un +)
            bool hadPlus = phone.Trim().StartsWith("+");
            string digits = new string(phone.Where(char.IsDigit).ToArray());

            // code pays
            string? cc = (countryIso ?? "FR").Trim().ToUpperInvariant() switch
            {
                "FR" or "FRA" => "33",
                "BE" or "BEL" => "32",
                "CH" or "CHE" => "41",
                _ => null
            };

            // déjà en +E164  -> on renvoie tel quel
            if (hadPlus && digits.Length >= 8 && digits.Length <= 15)
                return "+" + digits;

            // 0033xxxx -> +33xxxx
            if (digits.StartsWith("00") && digits.Length > 2)
                return "+" + digits.Substring(2);

            // 33xxxx (sans +) -> +33xxxx
            if (cc != null && digits.StartsWith(cc))
                return "+" + digits;

            // formats nationaux
            if (cc != null)
            {
                // 0XXXXXXXXX -> +33XXXXXXXXX
                if (digits.Length >= 10 && digits[0] == '0')
                    return "+" + cc + digits.Substring(1);

                // 9 chiffres déjà sans le 0 -> +33XXXXXXXXX
                if (digits.Length == 9)
                    return "+" + cc + digits;
            }

            // dernier recours : on ajoute simplement un +
            return "+" + digits;
        }


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
