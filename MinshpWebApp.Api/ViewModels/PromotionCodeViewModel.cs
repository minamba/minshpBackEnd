namespace MinshpWebApp.Api.ViewModels
{
    public class PromotionCodeViewModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? Purcentage { get; set; }

        public DateTime? DateCreation { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public int? GeneralCartAmount { get; set; }

        public bool IsUsed { get; set; }

        public ProductVIewModel Product { get; set; }
        public CategoryViewModel Category { get; set; }
        public SubCategoryViewModel SubCategory { get; set; }
    }
}
