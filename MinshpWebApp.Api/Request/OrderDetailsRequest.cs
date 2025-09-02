namespace MinshpWebApp.Api.Request
{
    public class OrderDetailsRequest
    {
        //expediteur
        public string? SenderCountry { get; set; }
        public string? SenderZipCode { get; set; }
        public string? SenderCity { get; set; }
        public string? SenderType { get; set; }


        //destinataire
        public string RecipientCountry { get; set; }
        public string? RecipientZipCode { get; set; }
        public string? RecipientCity { get; set; }
        public string? RecipientType { get; set; }

        //colis
        public string? PackageWeight { get; set; }
        public string? PackageLonger { get; set; }
        public string? PackageWidth { get; set; }
        public string? PackageHeight { get; set; }
        public decimal? PackageValue { get; set; }

        //code contenue
        public string? ContainedCode { get; set; }

    }
}
