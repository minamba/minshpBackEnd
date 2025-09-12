namespace MinshpWebApp.Api.Utils
{
    using QuestPDF.Fluent;
    using QuestPDF.Helpers;
    using QuestPDF.Infrastructure;

    public class TestPdf : IDocument
    {
        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Content().Text("✅ Test PDF généré avec succès !");
            });
        }
    }
}
