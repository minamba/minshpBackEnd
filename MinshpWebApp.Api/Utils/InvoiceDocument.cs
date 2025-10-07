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
        public  int Id { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Reduction { get; set; }
        public decimal TotalPriceItemProduct { get; set; }
    }

    public class InvoiceDocument : IDocument
    {
        public string InvoiceNumber { get; set; }
        public string OrderNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string BilledTo { get; set; }
        public string ShippedTo { get; set; }
        public List<InvoiceItem> Items { get; set; } = new();
        public decimal TotalHT { get; set; }
        public decimal TVA { get; set; }
        public decimal EcoPart { get; set; } = 0.00m;
        //public decimal TotalTTC => TotalHT + TVA + EcoPart;
        public decimal TotalTTC { get; set; }

        public string SocietyName { get; set; }
        public string SocietyAddress { get; set; }
        public string SocietyZipCode { get; set; }
        public string SocietyCity { get; set; }

        public decimal TVAPurcentage { get; set; }

      

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                // HEADER
                page.Header().Row(row =>
                {
                    // Bloc logo + adresse
                    row.ConstantItem(140).Column(col =>
                    {
                        col.Item().Element(x =>
                                x.Height(100).Image("wwwroot/Imgs/logo_facture.png", ImageScaling.FitHeight)
                            );

                    col.Item().PaddingTop(5).Text($"{SocietyName}").Bold();
                        col.Item().Text($"{SocietyAddress}");
                        col.Item().Text($"{SocietyZipCode}" + $" {SocietyCity}");
                    });

                    // Bloc facture aligné à droite
                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().AlignRight().Text("FACTURE").FontSize(14).Bold();
                        col.Item().AlignRight().Text($"N° de facture    : {InvoiceNumber}");
                        col.Item().AlignRight().Text($"Date                      : {InvoiceDate:dd/MM/yyyy}");
                        col.Item().AlignRight().Text($"Commande n° : {OrderNumber}");
                    });
                });

                // CONTENU PRINCIPAL
                page.Content().PaddingVertical(10).Column(col =>
                {
                    // Bloc clients
                    col.Item().AlignCenter().Row(row =>
                    {
                        row.RelativeItem().Border(1).Padding(10).Column(inner =>
                        {
                            inner.Item().Text("Facturation :").Bold();
                            inner.Item().Text(BilledTo);
                        });

                        row.ConstantItem(25); // Espace entre les 2 blocs

                        row.RelativeItem().Border(1).Padding(10).Column(inner =>
                        {
                            inner.Item().Text("Livraison :").Bold();
                            inner.Item().Text(ShippedTo);
                        });
                    });

                    col.Item().PaddingVertical(10); // Espacement

                    // Tableau des désignations
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4);  // Description
                            columns.ConstantColumn(70); // Qté
                            columns.ConstantColumn(70); // PU
                            columns.ConstantColumn(70); // REMISE
                            columns.ConstantColumn(70); // Total
                        });

                        // En-tête
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Désignation").Bold();
                            header.Cell().Element(CellStyle).AlignCenter().Text("Qté").Bold();
                            header.Cell().Element(CellStyle).AlignCenter().Text("PU HT €").Bold();
                            header.Cell().Element(CellStyle).AlignCenter().Text("Remise €").Bold();
                            header.Cell().Element(CellStyle).AlignCenter().Text("Total €").Bold();
                        });

                        // Lignes
                        foreach (var item in Items)
                        {
                            table.Cell().Element(CellStyle).Text(item.Description);
                            table.Cell().Element(CellStyle).AlignCenter().Text(item.Quantity.ToString());
                            table.Cell().Element(CellStyle).AlignRight().Text(item.UnitPrice.ToString("F2", CultureInfo.InvariantCulture));
                            table.Cell().Element(CellStyle).AlignRight().Text(item.Reduction.ToString("F2", CultureInfo.InvariantCulture));
                            table.Cell().Element(CellStyle).AlignRight().Text((item.TotalPriceItemProduct.ToString("F2", CultureInfo.InvariantCulture)));
                        }

                        static IContainer CellStyle(IContainer container) =>
                            container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    });

                    // Totaux à droite
                    col.Item().AlignRight().PaddingTop(20).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn();
                            cols.ConstantColumn(100);
                        });

                        table.Cell().Text("Montant HT (produits + livraison) :").Bold();
                        table.Cell().AlignRight().Text($"{TotalHT:F2} €");

                        table.Cell().Text("Eco-participation :");
                        table.Cell().AlignRight().Text($"{EcoPart:F2} €");

                        table.Cell().Text($"TVA {TVAPurcentage}% :");
                        table.Cell().AlignRight().Text($"{TVA:F2} €");

                        table.Cell().Text("Montant TTC :").Bold();
                        table.Cell().AlignRight().Text($"{TotalTTC:F2} €").Bold();
                    });
                });

                // FOOTER
                page.Footer().AlignCenter().Text("Merci pour votre commande - www.minshp.com");
            });
        }
    }
}
