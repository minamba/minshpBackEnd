using Azure.Core.GeoJson;
using System.Globalization;
using System.Text.Json;
using static System.Net.WebRequestMethods;
using System.Net.Http;

namespace MinshpWebApp.Api.Utils
{
    public class Geo
    {
        public sealed record GeoPoint(double Lat, double Lon);

        // Rayon moyen terrestre en km
        private const double R = 6371.0088;

        private static double ToRad(double deg) => (Math.PI / 180d) * deg;

        private static readonly HttpClient _http = new HttpClient();

        /// <summary>Distance great-circle en kilomètres.</summary>
        public static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        /// <summary>Distance en mètres (arrondie).</summary>
        public static int HaversineMeters(double lat1, double lon1, double lat2, double lon2)
            => (int)Math.Round(HaversineKm(lat1, lon1, lat2, lon2) * 1000.0);



        public async Task<GeoPoint?> GeocodeAsync(string address)
        {
            using var http = new HttpClient();
            // Nominatim (OpenStreetMap) – pas d’API key, mais il faut un User-Agent clair
            var url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(address)}";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.UserAgent.ParseAdd("Minshp/1.0 (contact: support@minshp.example)");
            var resp = await http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            var arr = doc.RootElement;
            if (arr.ValueKind != JsonValueKind.Array || arr.GetArrayLength() == 0) return null;

            var first = arr[0];
            if (!first.TryGetProperty("lat", out var latEl) || !first.TryGetProperty("lon", out var lonEl)) return null;

            double lat = double.Parse(latEl.GetString()!.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            double lon = double.Parse(lonEl.GetString()!.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
            return new GeoPoint(lat, lon);
        }


        public static async Task<GeoPoint?> GeocodeWithNominatimAsync(
        string address,
        string userAgent = "Minshp/1.0 (contact: support@minshp.example)",
        string countryCodes = "fr",
        int limit = 1)
        {
            if (string.IsNullOrWhiteSpace(address)) return null;

            var url = $"https://nominatim.openstreetmap.org/search" +
                      $"?format=json&addressdetails=0&limit={limit}" +
                      $"&countrycodes={Uri.EscapeDataString(countryCodes)}" +
                      $"&q={Uri.EscapeDataString(address)}";

            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.UserAgent.ParseAdd(userAgent);           // OBLIGATOIRE
            req.Headers.Referrer = new Uri("https://minshp.example"); // recommandé

            using var resp = await _http.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;

            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
                return null;

            var first = root[0];
            if (!first.TryGetProperty("lat", out var latEl) ||
                !first.TryGetProperty("lon", out var lonEl))
                return null;

            double lat = double.Parse(latEl.GetString()!.Replace(',', '.'), CultureInfo.InvariantCulture);
            double lon = double.Parse(lonEl.GetString()!.Replace(',', '.'), CultureInfo.InvariantCulture);
            return new GeoPoint(lat, lon);
        }
    }

}
