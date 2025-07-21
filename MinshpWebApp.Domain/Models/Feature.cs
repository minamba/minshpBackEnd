using System;
using System.Collections.Generic;

namespace MinshpWebApp.Domain.Models;

public partial class Feature
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public int? IdCategory { get; set; }
}
