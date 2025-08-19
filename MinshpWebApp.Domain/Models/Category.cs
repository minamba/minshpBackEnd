using System;
using System.Collections.Generic;

namespace MinshpWebApp.Domain.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? Name { get; set; }
    public string? IdTaxe { get; set; }
    public int? IdPromotionCode { get; set; }
}
