using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MinshpWebApp.Domain.Models
{
    public sealed class V3TrackingEvent
    {
        [JsonPropertyName("timestamp")]
        public DateTimeOffset? Timestamp { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("content")]
        public List<V3Package> Content { get; set; } = new();
    }

    public sealed class V3Package
    {
        [JsonPropertyName("packageId")]
        public string? PackageId { get; set; }

        [JsonPropertyName("packageExternalId")]
        public string? PackageExternalId { get; set; }

        [JsonPropertyName("trackingNumber")]
        public string? TrackingNumber { get; set; }

        [JsonPropertyName("packageTrackingUrl")]
        public string? PackageTrackingUrl { get; set; }

        // L'API envoie une string (ex: "2025-09-20 08:00:00")
        [JsonPropertyName("trackingDateTime")]
        public string? TrackingDateTime { get; set; }

        [JsonIgnore]
        public DateTimeOffset? TrackingDateTimeParsed =>
            DateTimeOffset.TryParse(TrackingDateTime, out var dt) ? dt : null;

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("isFinal")]
        public bool IsFinal { get; set; }

        [JsonPropertyName("history")]
        public List<V3History> History { get; set; } = new();
    }

    public sealed class V3History
    {
        [JsonPropertyName("trackingDateTime")]
        public string? TrackingDateTime { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("isFinal")]
        public bool IsFinal { get; set; }
    }
}
