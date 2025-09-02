using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class OrderCustomerProduct
{
    public int Id { get; set; }

    public int? OrderId { get; set; }

    public int? CustomerId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? ProductUnitPrice { get; set; }

    public virtual Order? Order { get; set; }
}
