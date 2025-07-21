namespace MinshpWebApp.Api.Enums
{
    using System.Text.Json.Serialization;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MediasType
    {
        IMAGE,

        VIDEO
    }
}
