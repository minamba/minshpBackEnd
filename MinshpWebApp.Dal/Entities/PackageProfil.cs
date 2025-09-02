using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class PackageProfil
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Longer { get; set; }

    public int? Width { get; set; }

    public int? Height { get; set; }

    public decimal? Weight { get; set; }

    public string? Description { get; set; }
}
