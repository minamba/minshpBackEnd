namespace MinshpWebApp.Api.ViewModels
{
    public class SubCategoryViewModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? IdPromotionCode { get; set; }

        public int? IdCategory { get; set; }

        public string? IdTaxe { get; set; }

        public List<string>? TaxeName { get; set; }

        public int? ContentCode { get; set; }
        public bool? Display { get; set; }

        public IEnumerable<PromotionCodeViewModel> PromotionCodes { get; set; }

    }
}
