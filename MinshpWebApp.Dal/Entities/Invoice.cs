using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class Invoice
{
    public int Id { get; set; }

    public string? InvoiceNumber { get; set; }

    public int? OrderId { get; set; }

    public int? CustomerId { get; set; }

    public string? Representative { get; set; }

    public DateTime? DateCreation { get; set; }

    public long InvoiceNumberInt { get; set; }

    public string? InvoiceNumber1 { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Order? Order { get; set; }
}
