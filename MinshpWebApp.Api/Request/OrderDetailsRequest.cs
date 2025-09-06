using MinshpWebApp.Api.Enums;
using System.Text.Json.Serialization;

namespace MinshpWebApp.Api.Request
{
    public class OrderDetailsRequest
    {
        //expediteur
        public string? SenderCountry { get; set; }
        public string? SenderZipCode { get; set; }
        public string? SenderCity { get; set; }
        public CustomerEnum? SenderType { get; set; } = CustomerEnum.ENTREPRISE;


        //destinataire
        public string? RecipientCountry { get; set; }
        public string? RecipientZipCode { get; set; }
        public string? RecipientCity { get; set; }
        public CustomerEnum? RecipientType { get; set; } = CustomerEnum.PARTICULIER;

        //colis
       public List<PackageRequest>? Packages { get; set; } = new();

        //code contenue
        public string? ContainedCode { get; set; }

    }

    public class PackageRequest
    {
        public string? Id { get; set; }

        public string? ContainedCode { get; set; }

        public string? PackageWeight { get; set; }

        public string? PackageLonger { get; set; }

        public string? PackageWidth { get; set; }

        public string? PackageHeight { get; set; }
        public decimal? PackageValue { get; set; }
    }

}
