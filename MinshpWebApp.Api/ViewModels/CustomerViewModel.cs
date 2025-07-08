namespace MinshpWebApp.Api.ViewModels
{
    public class CustomerViewModel
    {
        public int Id { get; set; }

        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public string? Password { get; set; }

        public string? PhoneNumber { get; set; }

        public string? DeliveryAddress { get; set; }

        public string? BillingAddress { get; set; }

        public IEnumerable<OrderViewModel> Orders { get; set; }
    }
}
