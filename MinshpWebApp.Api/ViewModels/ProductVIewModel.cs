namespace MinshpWebApp.Api.ViewModels
{
    public class ProductVIewModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }
        public string? Category { get; set; }
        public bool ? Main { get; set; }
        public string? Brand { get; set; }

        public string? Model { get; set; }
        public DateTime? CreationDate { get; set; }

        public DateTime? ModificationDate { get; set; }
        public IEnumerable<PromotionViewModel>? Promotions { get; set; }
        public IEnumerable<FeatureViewModel>? Features { get; set; }
        public IEnumerable<VideoViewModel>? Videos { get; set; }
        public IEnumerable<ImageViewModel>? Images { get; set; }
        public StockViewModel? Stocks { get; set; }
    }
}
