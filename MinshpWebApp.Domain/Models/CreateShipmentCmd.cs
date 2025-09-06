using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class CreateShipmentCmd
    {
        public string? ServiceCode { get; set; }
        public bool? IsRelay { get; set; }
        public string? RelayId { get; set; }
        public string? ToFirstName { get; set; }
        public string? ToLastName { get; set; }
        public string? ToStreet { get; set; }
        public string? ToExtra { get; set; }
        public string? ToZip { get; set; }
        public string? ToCity { get; set; }
        public string? ToCountry { get; set; }
        public decimal? WeightKg { get; set; }
        public decimal? DeclaredValue { get; set; }


        public bool? Insured { get; set; } = false;
        public string? ContentId { get; set; }            // ex "content:v1:10150"
        public string? ContentDescription { get; set; }
        public string? PackageExternalId { get; set; }
        public string? OrderExternalId { get; set; }      // ex: OrderId en string
        public int? PackageWidth { get; set; }
        public int? PackageHeight { get; set; }
        public int? PackageLength { get; set; }
        public string? ToEmail { get; set; }
        public string? ToPhone { get; set; }
        public int? ToNumber { get; set; }
        public string? LabelType { get; set; }            // "PDF_A4" par défaut
        public string? ShippingOfferId { get; set; }
        public string? ExpectedTakingOverDate { get; set; } // ou DateTime?
        public string? DropOffPointCode { get; set; }


        public Shipment? Shipment { get; set; }


    }


    public class Shipment
    {
        public int? OrderId { get; set; }
        public List<Package>? Packages { get; set; }
        public FromAddress? FromAddress { get; set; }
        public ToAddress? ToAddress { get; set; }
        public ReturnAddress? ReturnAddress { get; set; }
        public string? ExternalId { get; set; }
        public string? PickupPointCode { get; set; }
        public string? DropOffPointCode { get; set; }
    }

    public class ToAddress
    {
        public string? Type { get; set; } = "particulier";
        public Contact? Contact { get; set; }
        public Location? Location { get; set; }

    }
    public class FromAddress
    {
        public string? Type { get; set; } = "entreprise";
        public Contact? Contact { get; set; }
        public Location? Location { get; set; }
    }


    public class Location
    {
        public string? City { get; set; }
        public string? Number { get; set; }
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
        public string? CountryIsoCode { get; set; } = "FR";
    }

    public class ReturnAddress
    {
        public string? Type { get; set; } = "entreprise";
        public Contact? Contact { get; set; }
        public Location? Location { get; set; }
    }


    public class Contact
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
    }


}
