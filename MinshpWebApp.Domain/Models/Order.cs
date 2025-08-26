using System;
using System.Collections.Generic;

namespace MinshpWebApp.Domain.Models;

public partial class Order
{
    public int Id { get; set; }

    public string? OrderNumber { get; set; }

    public int? Quantity { get; set; }

    public DateTime? Date { get; set; }

    public string? Status { get; set; }

    public int? IdCustomer { get; set; }

    public int? IdProduct { get; set; }


    public string? PaymentMethod { get; set; }

    public decimal? Amount { get; set; }

    public string? OrderNumber1 { get; set; }

}
