using System;
using System.Collections.Generic;

namespace MinshpWebApp.Domain.Models;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }

    public int? IdCategory { get; set; }

    public bool? Main { get; set; }

    public string? Url { get; set; }
}
