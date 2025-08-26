namespace MinshpWebApp.Api.Request
{
    public class InvoiceRequest
    {
        public int? Id { get; set; }

        public string? InvoiceNumber { get; set; }

        public int? OrderId { get; set; }

        public int? CustomerId { get; set; }

        public string? Representative { get; set; }

        public DateTime? DateCreation { get; set; }

        public string? InvoiceNumber1 { get; set; }
    }
}
