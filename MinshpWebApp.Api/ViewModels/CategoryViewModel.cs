namespace MinshpWebApp.Api.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? TaxeId { get; set; }
        public List<string>? TaxeName { get; set; }
        public int? IdPromotionCode { get; set; }
        public int? IdPackageProfil { get; set; }
        public PackageProfilViewModel? PackageProfil { get; set; }
    }
}
