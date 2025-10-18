namespace MinshpWebApp.Api.Request
{
    public class CustomerRateRequest
    {
        public int? Id { get; set; }

        public int? IdCustomer { get; set; }

        public int? IdProduct { get; set; }

        public int? Rate { get; set; }

        public string? Title { get; set; }

        public string? Message { get; set; }
    }
}