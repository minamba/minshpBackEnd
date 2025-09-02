namespace MinshpWebApp.Api.ViewModels
{
    public class ProductVIewModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }
        public decimal? PriceTtc { get; set; }
        public decimal? PriceTtcPromoted { get; set; }
        public decimal? PriceTtcCategoryCodePromoted { get; set; }
        public int? PurcentageCodePromoted { get; set; }
        public string? TaxWithoutTvaAmount { get; set; } // montant de la taxe hors TVA. en general on additionne la TVA + la taxe supplémentaire
        public string? Category { get; set; }
        public bool ? Main { get; set; }
        public string? Brand { get; set; }

        public string? Model { get; set; }
        public string ? StockStatus { get; set; }
        public DateTime? CreationDate { get; set; }

        public DateTime? ModificationDate { get; set; }
        public int? IdPromotionCode { get; set; }
        public int? IdPackageProfil { get; set; }
        public PackageProfilViewModel? PackageProfil { get; set; }
        public IEnumerable<PromotionViewModel>? Promotions { get; set; }
        public IEnumerable<FeatureViewModel>? Features { get; set; }
        public IEnumerable<VideoViewModel>? Videos { get; set; }
        public IEnumerable<ImageViewModel>? Images { get; set; }
        public StockViewModel? Stocks { get; set; }
    }
}
