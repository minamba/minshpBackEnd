namespace MinshpWebApp.Api.Request
{
    public class NewLetterRequest
    {
        public int? Id { get; set; }

        public string? Mail { get; set; }
        public string? OldMAil { get; set; }
        public bool? Suscribe { get; set; }

    }
}
