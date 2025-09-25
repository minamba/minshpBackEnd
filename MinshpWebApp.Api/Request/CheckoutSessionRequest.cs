using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Request
{
    public class CheckoutSessionRequest
    {


        //V3 COMMANDE BOXTAL *******************************************************************************************************
        public bool? Insured { get; set; } = false;
        public string? ExternalId { get; set; }
        public string? LabelType { get; set; } = "PDF_A4";
        public string? ShippingOfferId { get; set; }
        public string? ShippingOfferCode { get; set; }
        public DateTime? ExpectedTakingOverDate { get; set; }

        public  MinshpWebApp.Domain.Models.Shipment? Shipment { get; set; }

        //Infos complementaire 
        public List<PromoUseCode>? UseCodes { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerNumber { get; set; }

        public DateTime? Date { get; set; }

        public string? Status { get; set; }

        public string? PaymentMethod { get; set; }

        public decimal? Amount { get; set; }

        public decimal? DeliveryAmount { get; set; }
        public string? ContentDescription { get; set; }

        public List<OrderCustomerProductRequest> OrderCustomerProducts { get; set; }
        public string ContentCode { get; set; } = "50110";

        //V3 COMMANDE BOXTAL *******************************************************************************************************




        // REDIRECTION VERS LES PAGES DE SUCCES CANCEL OU ERREUR EN FONCTION DE L'ETAT
        public string? SuccessUrl { get; set; }
        public string? CancelUrl { get; set; }
        public string? ErrorUrl { get; set; }




        //++++++++++++++++++++** CREATION DE LA COMMANDE ***********************************************************
        //public int? CustomerId { get; set; }

        //public DateTime? Date { get; set; }

        //public string? Status { get; set; }

        //public string? PaymentMethod { get; set; }

        //public decimal? Amount { get; set; }

        //public decimal? DeliveryAmount { get; set; }



        //PARTIE SHIPPING CONTENU DANS ORDER
        public string? DeliveryMode { get; set; } = "home";   // "home" | "relay"
        public string? Carrier { get; set; } = "";            // ex: "Chronopost"
        public string? RelayId { get; set; }                 // id point relais
        public string? RelayLabel { get; set; }              // libellé point relais
        public string? BoxtalShipmentId { get; set; }        // id envoi Boxtal
        public string? TrackingNumber { get; set; }          // n° suivi
        public string? LabelUrl { get; set; }             // URL étiquette

        public string? TrackingLink { get; set; }





        //********************* CREATION DE ORDER PRODUCT *******************************************************

        //public List<OrderCustomerProductRequest> OrderCustomerProducts { get; set; }


        //+++++++++++++++++++++ CREATION DE LA LIVRAISON POUR BOXTAL +++++++++++++++++++++++++++++++++++++++++++++

        // Points relais choisis (issus de mandatory_informations dans la réponse de cotation)
        public string OperatorCode { get; set; } = "";        // ex: "CHRP"
        public string ServiceCode { get; set; } = "";        // ex: "ChronoShoptoShop"

        //public string ContentCode { get; set; } = "50110";

        public bool IsRelay { get; set; } = false;
        public string? UrlPush { get; set; }

        // Points relais choisis (issus de mandatory_informations dans la réponse de cotation)
        public string? DropOffPointCode { get; set; }         // ex: "CHRP-076AV" (depot.pointrelais)
        public string? PickupPointCode { get; set; }         // ex: "CHRP-617VA" (retrait.pointrelais)

        // Contenu / valeur
        //public string? ContentDescription { get; set; }       // ex: "Livre illustré pour enfants"
        public decimal? DeclaredValue { get; set; }       // ex: 30

        // Colis (tu peux réutiliser ta classe Package pour poids/dims/valeur)
        public List<Package> Packages { get; set; } = new();

        //liste des codes utilisés dans le panier
        //public List<PromoUseCode>? UseCodes { get; set; }

        // Destinataire
        //public string ToType { get; set; } = "particulier"; // "entreprise"|"particulier"
        //public string? ToCivility { get; set; }                  // ex: "M","Mme" (si exigé par l'offre)
        //public string? ToLastName { get; set; }
        //public string? ToFirstName { get; set; }
        //public string? ToEmail { get; set; }
        //public string? ToPhone { get; set; }
        //public string? ToAddress { get; set; }                  // ligne d'adresse
        //public string? ToZip { get; set; }
        //public string? ToCity { get; set; }
        //public string? ToCountry { get; set; } = "FR";


        // Divers
        public DateTime? TakeOverDate { get; set; }              // date de dépôt souhaitée (si exigée)
        public string? ExternalOrderId { get; set; }             // pour suivi côté BO

        public int? OrderId { get; set; }

    }
}
