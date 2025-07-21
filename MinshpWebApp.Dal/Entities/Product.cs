using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? Id_Category { get; set; }

    public bool? Main { get; set; }

    public virtual Category? IdCategoryNavigation { get; set; }

    public virtual ICollection<ProductFeature> ProductFeatures { get; set; } = new List<ProductFeature>();

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}
