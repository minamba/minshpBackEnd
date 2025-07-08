namespace MinshpWebApp.Api.Request
{
    public class PromotionRequest
    {
        public int? Id { get; set; }

        public int? Purcentage { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? Id_product { get; set; }
    }
}
