namespace MinshpWebApp.Api.ViewModels
{
    public class InvoiceViewModel
    {
        public int Id { get; set; }

        public int? OrderId { get; set; }

        public int? CustomerId { get; set; }

        public DateTime? DateCreation { get; set; }

        public string? Representative { get; set; }

        public string? InvoiceNumber { get; set; }

        public CustomerViewModel Customer { get; set; }

        public OrderViewModel Order { get; set; }

    }
}
