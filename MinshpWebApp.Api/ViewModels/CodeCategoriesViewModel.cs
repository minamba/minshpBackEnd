namespace MinshpWebApp.Api.ViewModels
{
    public class CodeCategoryViewModel
    {
        public string Id { get; set; }
        public string Label { get; set; }
    }

    public class CodeCategoriesViewModel
    {
        public List<CodeCategoryViewModel> AllCodeCategories { get; set; } = new List<CodeCategoryViewModel>();
    }
}
