namespace MinshpWebApp.Api.Request
{
    public class StockRequest
    {
        public int? Id { get; set; }

        public int? Quantity { get; set; }
        public int? IdProduct { get; set; }
    }
}
