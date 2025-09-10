namespace MinshpWebApp.Api.ViewModels
{
    public class RateViewModel
    {
        public string? Code { get; set; }
        public string? Label { get; set; }
        public string? Carrier { get; set; }

        public decimal? PriceTtc { get; set; }

        public bool? IsRelay { get; set; }

        public List<string> DropOffPointCodes { get; set; }
        public List<string> PickupPointCodes { get; set; }
    }
}
