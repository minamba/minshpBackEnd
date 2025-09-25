using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class OrderDetails
    {
        //expediteur
        public string SenderCountry { get; set; }
        public string SenderZipCode { get; set; }
        public string SenderCity { get; set; }
        public string SenderType { get; set; }


        //destinataire
        public string RecipientCountry { get; set; }
        public string RecipientZipCode { get; set; }
        public string RecipientCity { get; set; }
        public string RecipientType { get; set; }


        //colis
        public IEnumerable<Package> Packages { get; set; }
        

        //code contenue
        public string ContainedCode { get; set; }
    }



    public class Package
    {
        public string? Type { get; set; } = "PARCEL";
        public string? PackageId { get; set; }
        public String ContentCodeDefault { get; set; } = "50110";

        public PackageValue Value { get; set; }
        public PackageContent Content { get; set; }
        public string? PackageWeight { get; set; }
        public string? PackageLonger { get; set; }
        public string? PackageWidth { get; set; }
        public string? PackageHeight { get; set; }
        public decimal? PackageValue { get; set; }
        public bool? PackageStackable { get; set; } = true;
        public string? ExternalId { get; set; }
    }


    public class  PackageValue
    {
        public decimal? Value { get; set; }
        public string? Currency { get; set; } = "Eur";
    }

    public class PackageContent
    {
        public string? Id { get; set; }
        public string? Description { get; set; }
    }


    public class PromoUseCode
    {
        public string? Id { get; set; }
        public string? Name { get; set; }

    }
}
