using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class Feature
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public int? IdCategory { get; set; }

    public virtual Category? IdCategoryNavigation { get; set; }

    public virtual ICollection<ProductFeature> ProductFeatures { get; set; } = new List<ProductFeature>();
}
