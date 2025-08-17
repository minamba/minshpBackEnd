namespace MinshpWebApp.Api.Request
{
    public class TaxeRequest
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public int? Purcentage { get; set; }

        public decimal? Amount { get; set; }
    }
}
