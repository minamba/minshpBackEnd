using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class DeliveryAddress
{
    public int Id { get; set; }

    public string? Address { get; set; }

    public string? ComplementaryAddress { get; set; }

    public int? PostalCode { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public bool? Favorite { get; set; }

    public int? IdCustomer { get; set; }

    public virtual Customer? IdCustomerNavigation { get; set; }
}
