namespace MinshpWebApp.Api.Request
{
    public class PromotionCodeRequest
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public int? Purcentage { get; set; }

        public DateTime? DateCreation { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsUsed { get; set; }

        public int? IdProduct { get; set; }
        public int? IdCategory { get; set; }
    }
}
