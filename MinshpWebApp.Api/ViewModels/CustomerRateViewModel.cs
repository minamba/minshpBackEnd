namespace MinshpWebApp.Api.ViewModels
{
    public class CustomerRateViewModel
    {
        public int Id { get; set; }

        public int? IdCustomer { get; set; }

        public int? IdProduct { get; set; }

        public int? Rate { get; set; }

        public string? Title { get; set; }

        public string? Message { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? ModificationDate { get; set; }

        public CustomerViewModel customer { get; set; }
        public ProductVIewModel product { get; set; }

    }
}
