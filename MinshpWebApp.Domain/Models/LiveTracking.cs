using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class LiveTracking
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonPropertyName("content")]
        public SubscriptionContent Content { get; set; } = new();

        [JsonPropertyName("contents")]
        public List<SubscriptionContent> Contents { get; set; } = new();
    }

    public class SubscriptionContent
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        // L'API peut renvoyer "appId" ou "appID" : avec System.Text.Json en mode
        // PropertyNameCaseInsensitive = true, "AppId" couvrira les deux.
        [JsonPropertyName("appId")]
        public string AppId { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("eventType")]
        public string EventType { get; set; } = string.Empty;

        [JsonPropertyName("callbackUrl")]
        public string CallbackUrl { get; set; } = string.Empty;

        [JsonPropertyName("webhookSecret")]
        public string WebhookSecret { get; set; } = string.Empty;
    }
}
