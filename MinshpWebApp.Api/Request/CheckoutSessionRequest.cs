using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Request
{
    public class CheckoutSessionRequest
    {
        //++++++++++++++++++++** CREATION DE LA COMMANDE ***********************************************************
        public int? CustomerId { get; set; }

        public DateTime? Date { get; set; }

        public string? Status { get; set; }

        public string? PaymentMethod { get; set; }

        public decimal? Amount { get; set; }

        public decimal? DeliveryAmount { get; set; }



        //PARTIE SHIPPING CONTENU DANS ORDER
        public string? DeliveryMode { get; set; } = "home";   // "home" | "relay"
        public string? Carrier { get; set; } = "";            // ex: "Chronopost"
        public string? RelayId { get; set; }                 // id point relais
        public string? RelayLabel { get; set; }              // libellé point relais
        public string? BoxtalShipmentId { get; set; }        // id envoi Boxtal
        public string? TrackingNumber { get; set; }          // n° suivi
        public string? LabelUrl { get; set; }             // URL étiquette

        public string? TrackingLink { get; set; }




        // facultatif : le front peut les laisser vides, on calcule via FrontendBaseUrl
        public string? SuccessUrl { get; set; }
        public string? CancelUrl { get; set; }



        //********************* CREATION DE ORDER PRODUCT *******************************************************

        public List<OrderCustomerProductRequest> OrderCustomerProducts { get; set; }


        //+++++++++++++++++++++ CREATION DE LA LIVRAISON POUR BOXTAL +++++++++++++++++++++++++++++++++++++++++++++

        // Points relais choisis (issus de mandatory_informations dans la réponse de cotation)
        public string OperatorCode { get; set; } = "";        // ex: "CHRP"
        public string ServiceCode { get; set; } = "";        // ex: "ChronoShoptoShop"

        public string ContentCode { get; set; } = "50110";

        public bool IsRelay { get; set; } = false;


        public string? UrlPush { get; set; }

        // Points relais choisis (issus de mandatory_informations dans la réponse de cotation)
        public string? DropOffPointCode { get; set; }         // ex: "CHRP-076AV" (depot.pointrelais)
        public string? PickupPointCode { get; set; }         // ex: "CHRP-617VA" (retrait.pointrelais)

        // Contenu / valeur
        public string? ContentDescription { get; set; }       // ex: "Livre illustré pour enfants"
        public decimal? DeclaredValue { get; set; }       // ex: 30

        // Colis (tu peux réutiliser ta classe Package pour poids/dims/valeur)
        public List<Package> Packages { get; set; } = new();

        //liste des codes utilisés dans le panier
        public List<PromoUseCode>? UseCodes { get; set; }

        // Destinataire
        public string ToType { get; set; } = "particulier"; // "entreprise"|"particulier"
        public string? ToCivility { get; set; }                  // ex: "M","Mme" (si exigé par l'offre)
        public string? ToLastName { get; set; }
        public string? ToFirstName { get; set; }
        public string? ToEmail { get; set; }
        public string? ToPhone { get; set; }
        public string? ToAddress { get; set; }                  // ligne d'adresse
        public string? ToZip { get; set; }
        public string? ToCity { get; set; }
        public string? ToCountry { get; set; } = "FR";


        // Divers
        public DateTime? TakeOverDate { get; set; }              // date de dépôt souhaitée (si exigée)
        public string? ExternalOrderId { get; set; }             // pour suivi côté BO

        public int? OrderId { get; set; }

    }

    //public class BasketItem
    //{
    //    public int ProductId { get; set; }
    //    public int Qty { get; set; }
    //    public decimal UnitPriceTtc { get; set; }
    //}

    //public class ShipmentContext
    //{
    //    public List<Package> Packages { get; set; } = new(); // tu réutilises ton modèle Domain.Models.Package si tu veux
    //    public decimal CartWeightKg { get; set; }
    //    public decimal BaseTotal { get; set; }               // total produits TTC
    //}

    //public class ShippingSelection
    //{
    //    // "home" | "relay"
    //    public string Mode { get; set; } = "home";
    //    public decimal Price { get; set; }
    //    public string? RateCode { get; set; }
    //    public string? Carrier { get; set; }                 // libellé ex. "Chronopost"
    //    public string? OperatorCode { get; set; }            // ex. CHRP (rempli côté back si null)
    //    public string? DropOffPointCode { get; set; }        // utile pour shop-to-shop

    //    public RelayInfo? Relay { get; set; }
    //    public HomeAddressInfo? HomeAddress { get; set; }
    //}

    //public class RelayInfo
    //{
    //    public string? Id { get; set; }
    //    public string? Name { get; set; }
    //    public string? Address { get; set; }
    //    public string? Zip { get; set; }
    //    public string? City { get; set; }
    //    public string? Network { get; set; }                 // ex. CHRP/MONR/UPSE/POFR
    //}

    //public class HomeAddressInfo
    //{
    //    public string? Address { get; set; }
    //    public string? Zip { get; set; }
    //    public string? City { get; set; }
    //    public string? Country { get; set; } = "FR";
    //}


    // si tu as déjà Domain.Models.Package, garde-le ; sinon ce petit DTO suffit
    //public class PackageCheckout
    //{
    //    public string? Id { get; set; }
    //    public string? ContainedCode { get; set; }
    //    public string? PackageWeight { get; set; }
    //    public string? PackageLonger { get; set; }
    //    public string? PackageWidth { get; set; }
    //    public string? PackageHeight { get; set; }
    //    public decimal? PackageValue { get; set; }
    //    public bool PackageStackable { get; set; } = true;
    //    public string Type { get; set; } = "PARCEL";
    //}



    //POUR LA LIVRAISON 

}
