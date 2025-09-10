namespace MinshpWebApp.Api.Utils
{
    public static class TrackingLink
    {
        /// <summary>
        /// Construit une URL publique de suivi selon le transporteur.
        /// </summary>
        /// <param name="carrierCode">Ex. "CHRP", "MONR", "POFR", "UPSE", "DHLE", "FEDX", "SOGP", "COPR". 
        /// Si tu n'as que le shippingOfferCode, prends le préfixe avant le tiret: "CHRP-Chrono18" → "CHRP".</param>
        /// <param name="trackingNumber">Numéro de suivi retourné par Boxtal / le transporteur.</param>
        /// <param name="recipientPostalCode">Optionnel, utile pour Mondial Relay.</param>
        public static string? Build(string? carrierCode, string? trackingNumber, string? recipientPostalCode = null)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber)) return null;

            // Normalise : si tu as "CHRP-Chrono18" → "CHRP"
            var code = (carrierCode ?? "").Trim();
            var dash = code.IndexOf('-');
            if (dash > 0) code = code.Substring(0, dash);
            code = code.ToUpperInvariant();

            string esc(string s) => Uri.EscapeDataString(s);

            switch (code)
            {
                case "CHRP": // Chronopost
                    return $"https://www.chronopost.fr/tracking-no-cms/suivi-page?listeNumerosLT={esc(trackingNumber)}&langue=fr_FR";

                case "MONR": // Mondial Relay (souvent 8/10/12 chiffres) + CP optionnel
                    {
                        var cp = string.IsNullOrWhiteSpace(recipientPostalCode) ? "" : $"&CodePostal={esc(recipientPostalCode)}";
                        return $"https://www.mondialrelay.fr/suivi-de-colis/?NumeroExpedition={esc(trackingNumber)}{cp}";
                    }

                case "POFR":
                    return $"https://www.laposte.fr/suivi?code={esc(trackingNumber)}";

                case "UPSE": // UPS
                    return $"https://www.ups.com/track?HTMLVersion=5.0&loc=fr_FR&Requester=UPSHome&WBPM_lid=homepage%252Fct1.html_pnl_trk&track.x=Suivi&trackNums={esc(trackingNumber)}";


                //case "DHLE": // DHL Express
                //case "DHL":
                //    return $"https://www.dhl.com/fr-fr/home/suivi.html?tracking-id={esc(trackingNumber)}";

                //case "FEDX": // FedEx
                //case "FEDEX":
                //    return $"https://www.fedex.com/fedextrack/?trknbr={esc(trackingNumber)}&cntry_code=fr";

                //case "SOGP": // Relais Colis
                //    return $"https://www.relaiscolis.com/suivi-de-colis?num={esc(trackingNumber)}";

                //case "COPR": // Colis Privé
                //    return $"https://www.colisprive.com/mon-colis/suivi-de-colis?numero={esc(trackingNumber)}";

                default:
                    // Fallback best-effort
                    return $"https://www.google.com/search?q={esc(code + " " + trackingNumber)}";
            }
        }
    }

}
