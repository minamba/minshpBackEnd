namespace MinshpWebApp.Api.Request
{
    public class ApplicationRequest
    {
        public int? Id { get; set; }

        public int? DisplayNewProductNumber { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
