using System;
using System.Collections.Generic;


namespace MinshpWebApp.Dal.Entities;

public partial class Taxe
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Purcentage { get; set; }

    public decimal? Amount { get; set; }
}
