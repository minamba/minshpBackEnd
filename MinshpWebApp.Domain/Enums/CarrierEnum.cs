using System.Text.Json.Serialization;

namespace MinshpWebApp.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CarrierEnum
    {
        MONR,
        CHRP,
        UPSE,
        POFR
    }
}
