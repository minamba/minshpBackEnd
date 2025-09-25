using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    //POUR LA V3
    public class CreateShipmentCmd
    {
        public bool? Insured { get; set; } = false;
        public string? ExternalId { get; set; }
        public string? LabelType { get; set; } = "PDF_A4";
        public string? ShippingOfferId { get; set; }
        public string? ShippingOfferCode { get; set; }
        public DateTime? ExpectedTakingOverDate { get; set; }

        public Shipment? Shipment { get; set; }


        //Additionnal informations
        public string? OperatorCode { get; set; } = "";        // ex: "CHRP"
        public string? ServiceCode { get; set; } = "";        // ex: "ChronoShoptoShop"
        public string? PickupPointCode { get; set; }


    }


    public class Shipment
    {
        public int? OrderId { get; set; }
        public List<Package>? Packages { get; set; }
        public Address? FromAddress { get; set; }
        public Address? ToAddress { get; set; }
        public Address? ReturnAddress { get; set; }
        public string? ExternalId { get; set; }
        public string? PickupPointCode { get; set; }
        public string? DropOffPointCode { get; set; }
    }

    public class ToAddress
    {
        public string? Type { get; set; } = "particulier";
        public Contact? Contact { get; set; }
        public Location? Location { get; set; }
        public string? AdditionalInformation { get; set; }

    }
    public class Address
    {
        public string? Type { get; set; } = "entreprise";
        public Contact? Contact { get; set; }
        public Location? Location { get; set; }
        public string? AdditionalInformation { get; set; }
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
        public string? Civility { get; set; }
    }



    //POUR LA V1
    //public class CreateShipmentV1Cmd
    //{
    //    // Offre choisie (venant d'une des offres de /cotation V1)
    //    public string OperatorCode { get; set; } = "";        // ex: "CHRP"
    //    public string ServiceCode { get; set; } = "";        // ex: "ChronoShoptoShop"

    //    public string ContentCode { get; set; } = "50110";

    //    public bool IsRelay { get; set; } = false;


    //    public string? UrlPush { get; set; }

    //    // Points relais choisis (issus de mandatory_informations dans la réponse de cotation)
    //    public string? DropOffPointCode { get; set; }         // ex: "CHRP-076AV" (depot.pointrelais)
    //    public string? PickupPointCode { get; set; }         // ex: "CHRP-617VA" (retrait.pointrelais)

    //    // Contenu / valeur
    //    public string? ContentDescription { get; set; }       // ex: "Livre illustré pour enfants"
    //    public decimal? DeclaredValue { get; set; }       // ex: 30

    //    // Colis (tu peux réutiliser ta classe Package pour poids/dims/valeur)
    //    public List<Package> Packages { get; set; } = new();

    //    // Destinataire
    //    public string ToType { get; set; } = "particulier"; // "entreprise"|"particulier"
    //    public string? ToCivility { get; set; }                  // ex: "M","Mme" (si exigé par l'offre)
    //    public string? ToLastName { get; set; }
    //    public string? ToFirstName { get; set; }
    //    public string? ToEmail { get; set; }
    //    public string? ToPhone { get; set; }
    //    public string? ToAddress { get; set; }                  // ligne d'adresse
    //    public string? ToZip { get; set; }
    //    public string? ToCity { get; set; }
    //    public string? ToCountry { get; set; } = "FR";

    //    // Expéditeur (par défaut on prendra _opt.* si rien n'est fourni)
    //    public string FromType { get; set; } = "entreprise";
    //    public string? FromCivility { get; set; } = "M";
    //    public string? FromCompany { get; set; }
    //    public string? FromLastName { get; set; }
    //    public string? FromFirstName { get; set; }
    //    public string? FromEmail { get; set; }
    //    public string? FromPhone { get; set; }
    //    public string? FromAddress { get; set; }
    //    public string? FromZip { get; set; }
    //    public string? FromCity { get; set; }
    //    public string? FromCountry { get; set; } = "FR";

    //    // Divers
    //    public DateTime? TakeOverDate { get; set; }              // date de dépôt souhaitée (si exigée)
    //    public string? ExternalOrderId { get; set; }             // pour suivi côté BO

    //    public int? OrderId { get; set; }
    //}


}
