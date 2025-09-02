namespace MinshpWebApp.Api.Request
{
    public class PackageProfilRequest
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public int? Longer { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public decimal? Weight { get; set; }

        public string? Description { get; set; }
    }
}
