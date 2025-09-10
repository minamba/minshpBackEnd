namespace MinshpWebApp.Api.Request
{
    public class StripeRequest
    {
        public long Amount { get; set; }              // en centimes
        public string Currency { get; set; } = "eur";
        public string Description { get; set; } = "Commande";
        public string SuccessUrl { get; set; }        // ex: "https://.../checkout/success"
        public string CancelUrl { get; set; }
        public string CustomerEmail { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
