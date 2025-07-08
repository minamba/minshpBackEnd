namespace MinshpWebApp.Api.Request
{
    public class OrderRequest
    {
        public int? Id { get; set; }

        public Guid? OrderNumber { get; set; }

        public int? Quantity { get; set; }

        public DateTime? Date { get; set; }

        public string? Status { get; set; }

        public int? IdCustomer { get; set; }

        public int? Id_product { get; set; }
    }
}
