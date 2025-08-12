namespace MinshpWebApp.Api.Request
{
    public class FeatureRequest
    {
        public int? Id { get; set; }
        public string? Description { get; set; }
        public int? IdCategory { get; set; }
        public int? IdFeatureCategory { get; set; }
    }
}
