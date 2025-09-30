using MinshpWebApp.Api.ViewModels;

namespace MinshpWebApp.Api.Request
{
    public class CategoryRequest
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public string? IdTaxe { get; set; }
        public int? IdPromotionCode { get; set; }

        public int? IdPackageProfil { get; set; }

        public int? ContentCode { get; set; }

        public bool? Display { get; set; }
    }
}
