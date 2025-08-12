namespace MinshpWebApp.Api.ViewModels
{
    public class FeaturesCategoryProductViewModel
    {
        public int IdFeatureCategory { get; set; }
        public string? FeatureCategoryName { get; set; }
        public Dictionary<string,string> Specs { get; set; }
    }
}
