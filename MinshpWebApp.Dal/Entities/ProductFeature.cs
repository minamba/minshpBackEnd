using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class ProductFeature
{
    public int Id { get; set; }

    public int? IdProduct { get; set; }

    public int? IdFeature { get; set; }

    public virtual Feature? IdFeatureNavigation { get; set; }

    public virtual Product? IdProductNavigation { get; set; }
}
