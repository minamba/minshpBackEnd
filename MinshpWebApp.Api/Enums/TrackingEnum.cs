using System.Text.Json.Serialization;

namespace MinshpWebApp.Api.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TrackingEnum
    {
        ANNOUNCED,
        SHIPPED,
        IN_TRANSIT,
        OUT_FOR_DELIVERY,
        FAILED_ATTEMPT,
        REACHED_DELIVERY_PICKUP_POINT,
        DELIVERED,
        PENDING
    }
}
