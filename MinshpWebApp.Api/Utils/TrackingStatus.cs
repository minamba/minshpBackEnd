using MinshpWebApp.Api.Enums;

namespace MinshpWebApp.Api.Utils
{
    public static class TrackingStatus
    {
        public static string GetTrackingStatus(string sts)
        {
            string waiting = "En attente";
            string announced = "Envoie préparé";
            string shipped = "Envoyé";
            string inTransit = "En transit";
            string outOfDelivery = "En cours de livraison";
            string failedToAttempt = "Un problème a empêché la livraison";
            string delivered = "Livré";
            string currentlyInProcessing= "En cours de traitement";

            if (Enum.TryParse(typeof(TrackingEnum), sts, true, out var resultat) && Enum.IsDefined(typeof(TrackingEnum), resultat))
            {
                TrackingEnum getstatusFromShipping = Enum.Parse<TrackingEnum>(sts);


                string status = getstatusFromShipping switch
                {
                    TrackingEnum.ANNOUNCED => announced,
                    TrackingEnum.SHIPPED => shipped,
                    TrackingEnum.IN_TRANSIT => inTransit,
                    TrackingEnum.OUT_FOR_DELIVERY => outOfDelivery,
                    TrackingEnum.FAILED_ATTEMPT => failedToAttempt,
                    TrackingEnum.DELIVERED => delivered,
                    TrackingEnum.PENDING => waiting,
                    _ => waiting
                };


                return status;
            }
            else
                return currentlyInProcessing;
        }
    }
}
