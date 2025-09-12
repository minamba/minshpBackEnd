using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MinshpWebApp.Api.Utils
{
    public class InvoiceItem
    {
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class InvoiceDocument : IDocument
    {
        public string InvoiceNumber { get; set; }
        public string OrderNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string BilledTo { get; set; }
        public string ShippedTo { get; set; }
        public List<InvoiceItem> Items { get; set; }
        public decimal TotalHT { get; set; }
        public decimal TVA { get; set; }
        public decimal TotalTTC => TotalHT + TVA;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Column(col =>
                {
                    col.Item().Text("FACTURE").FontSize(20).Bold();
                    col.Item().Text($"N° de facture : {InvoiceNumber}");
                    col.Item().Text($"Date : {InvoiceDate:dd/MM/yyyy}");
                    col.Item().Text($"Commande n° : {OrderNumber}");
                });

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Client facturé :").Bold();
                            c.Item().Text(BilledTo);
                        });
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Client livré :").Bold();
                            c.Item().Text(ShippedTo);
                        });
                    });

                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(4);  // Description
                            cols.ConstantColumn(40); // Qté
                            cols.ConstantColumn(70); // PU
                            cols.ConstantColumn(70); // Total
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Désignation").Bold();
                            header.Cell().Text("Qté").Bold();
                            header.Cell().Text("PU €").Bold();
                            header.Cell().Text("Total €").Bold();
                        });

                        foreach (var item in Items)
                        {
                            table.Cell().Text(item.Description);
                            table.Cell().Text(item.Quantity.ToString());
                            table.Cell().Text(item.UnitPrice.ToString("F2", CultureInfo.InvariantCulture));
                            table.Cell().Text((item.UnitPrice * item.Quantity).ToString("F2", CultureInfo.InvariantCulture));
                        }
                    });

                    col.Item().AlignRight().Column(right =>
                    {
                        right.Item().Text($"Montant HT : {TotalHT:F2} €").Bold();
                        right.Item().Text($"TVA 20% : {TVA:F2} €");
                        right.Item().Text($"Montant TTC : {TotalTTC:F2} €").FontSize(12).Bold();
                    });
                });

                page.Footer().AlignCenter().Text("Merci pour votre commande - www.minshp.com");
            });
        }
    }
}
