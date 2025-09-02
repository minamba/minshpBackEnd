using System.Text.Json.Serialization;

namespace MinshpWebApp.Api.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DaysEnum
    {
        MONDAY,
        TUESDAY,
        WEDNESDAY,
        THURSDAY,
        FRIDAY,
        SATURDAY,
        SUNDAY
    }
}
