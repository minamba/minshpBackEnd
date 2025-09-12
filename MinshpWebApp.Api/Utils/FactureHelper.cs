using QuestPDF.Fluent;
using System.Globalization;

namespace MinshpWebApp.Api.Utils
{
    public static class FactureHelper
    {
        public static string SaveInvoicePdf(InvoiceDocument invoice)
        {
            var now = DateTime.Now;

            // Ex: "septembre_2025"
            var folderName = $"{CultureInfo.GetCultureInfo("fr-FR").DateTimeFormat.GetMonthName(now.Month)}_{now.Year}".ToLower();

            // Ex: "wwwroot/factures/septembre_2025"
            var folderPath = Path.Combine("wwwroot", "factures", folderName);

            // Crée le dossier s’il n'existe pas
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Nom du fichier : FV202509111234.pdf
            var fileName = $"{invoice.InvoiceNumber}.pdf";

            // Chemin final du fichier
            var fullPath = Path.Combine(folderPath, fileName);

            // Générer le PDF
            QuestPDF.Fluent.Document.Create(container => invoice.Compose(container))
                         .GeneratePdf(fullPath); // ✅

            return fullPath; // tu peux aussi retourner un chemin relatif si besoin
        }
    }
}
