namespace MinshpWebApp.Api.Request
{
    public class BillingAddressRequest
    {
        public int? Id { get; set; }

        public string? Address { get; set; }

        public string? ComplementaryAddress { get; set; }

        public int? PostalCode { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public int? IdCustomer { get; set; }
    }
}
