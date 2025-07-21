namespace MinshpWebApp.Api.ViewModels
{
    public class PromotionViewModel
    {
        public int Id { get; set; }

        public int? Purcentage { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int ? IdProduct { get; set; }

        public DateTime? DateCreation { get; set; }
    }
}
