namespace MinshpWebApp.Api.Request
{
    public class ProductRequest
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public int? IdCategory { get; set; }
    }
}
