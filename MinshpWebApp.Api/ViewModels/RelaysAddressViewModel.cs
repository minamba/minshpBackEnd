namespace MinshpWebApp.Api.ViewModels
{
    public class RelaysAddressViewModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? ZipCode { get; set; }
        public string City { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Distance { get; set; }
        public string? Network { get; set; }
        public string? Carrier { get; set; }
        public string? Schedules { get; set; }
    }
}
