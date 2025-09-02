using System.Text.Json.Serialization;

namespace MinshpWebApp.Api.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CarrierEnum
    {
        MONR,
        CHRP,
        UPSE,
    }
}
