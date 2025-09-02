namespace MinshpWebApp.Api.ViewModels
{
    public class OrderCustomerProductViewModel
    {
        public int Id { get; set; }

        public int? OrderId { get; set; }

        public int? CustomerId { get; set; }

        public int? ProductId { get; set; }

        public int? Quantity { get; set; }

        public decimal? ProductUnitPrice { get; set; }
    }
}
