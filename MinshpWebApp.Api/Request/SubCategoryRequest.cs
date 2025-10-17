namespace MinshpWebApp.Api.Request
{
    public class SubCategoryRequest
    {
        public int? Id { get; set; }

        public string? Name { get; set; }

        public int? IdPromotionCode { get; set; }

        public int? IdCategory { get; set; }

        public string? IdTaxe { get; set; }

        public int? ContentCode { get; set; }

        public bool? Display { get; set; }

        public int? IdImage { get; set; }
    }
}
