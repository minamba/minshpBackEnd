using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class Stock
{
    public int Id { get; set; }

    public int? Quantity { get; set; }

    public int? IdProduct { get; set; }

    public virtual Product? IdProductNavigation { get; set; }
}
