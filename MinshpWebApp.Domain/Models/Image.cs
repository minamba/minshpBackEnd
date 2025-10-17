using System;
using System.Collections.Generic;

namespace MinshpWebApp.Domain.Models;

public partial class Image
{
    public int Id { get; set; }

    public string? Url { get; set; }

    public string? Description { get; set; }

    public string? Title { get; set; }

    public int? IdProduct { get; set; }

    public int? Position { get; set; }

    public bool? Display { get; set; }
    public int? IdCategory { get; set; }
    public int? IdSubCategory { get; set; }

    public bool ComeFromCategory { get; set; }

    public bool ComeFromSubCategory { get; set; }
}
