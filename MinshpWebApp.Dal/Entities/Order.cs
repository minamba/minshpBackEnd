using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class Order
{
    public int Id { get; set; }

    public string? OrderNumber { get; set; }

    public int? Quantity { get; set; }

    public DateTime? Date { get; set; }

    public string? Status { get; set; }

    public int? IdCustomer { get; set; }

    public int? Id_product { get; set; }
    public string? PaymentMethod { get; set; }

    public decimal? Amount { get; set; }

    public long OrderNumberInt { get; set; }

    public string? OrderNumber1 { get; set; }

    public virtual Customer? IdCustomerNavigation { get; set; }
}
