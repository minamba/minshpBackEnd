using System;
using System.Collections.Generic;

namespace MinshpWebApp.Domain.Models;

public partial class Promotion
{
    public int Id { get; set; }

    public int? Purcentage { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? IdProduct { get; set; }

}
