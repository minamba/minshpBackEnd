namespace MinshpWebApp.Api.Request
{
    public class ImageRequest
    {
        public int? Id { get; set; }
        public string? Url { get; set; }

        public string? Description { get; set; }

        public int? IdProduct { get; set; }
    }
}
