using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class Application
{
    public int Id { get; set; }

    public int? DisplayNewProductNumber { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
