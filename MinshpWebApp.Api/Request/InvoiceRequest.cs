namespace MinshpWebApp.Api.Request
{
    public class InvoiceRequest
    {
        public int? Id { get; set; }

        public int? OrderId { get; set; }

        public int? CustomerId { get; set; }

        public string? Representative { get; set; }

        public string? InvoiceLink { get; set; }

        public bool? HardDelete { get; set; } = false;
    }
}
