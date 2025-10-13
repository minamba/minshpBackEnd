namespace MinshpWebApp.Api.ViewModels
{
    public class ProductFeatureViewModel
    {
        public int Id { get; set; }

        public int? IdProduct { get; set; }

        public int? IdFeature { get; set; }

        public string?  Category { get; set; }
        public string? Feature { get; set; }
        public string?  Product { get; set; }
    }
}
