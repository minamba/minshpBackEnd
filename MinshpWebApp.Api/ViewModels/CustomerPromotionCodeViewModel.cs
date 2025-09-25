namespace MinshpWebApp.Api.ViewModels
{
    public class CustomerPromotionCodeViewModel
    {
        public int? Id { get; set; }

        public int? IdCutomer { get; set; }

        public int? IdPromotion { get; set; }

        public bool? IsUsed { get; set; }
    }
}
