using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class Category
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? IdTaxe { get; set; }
    public int? IdPromotionCode { get; set; }

    public virtual ICollection<Feature> Features { get; set; } = new List<Feature>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
