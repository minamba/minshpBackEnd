using System;
using System.Collections.Generic;

namespace MinshpWebApp.Domain.Models;

public partial class ProductFeature
{
    public int Id { get; set; }

    public int? IdProduct { get; set; }

    public int? IdFeature { get; set; }
}
