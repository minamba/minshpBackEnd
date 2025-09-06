using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class SubCategory
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? IdPromotionCode { get; set; }

    public int? IdCategory { get; set; }

    public string? IdTaxe { get; set; }

    public int? ContentCode { get; set; }

    public virtual Category? IdCategoryNavigation { get; set; }

}
