using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class Customer
{
    public int Id { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? Password { get; set; }

    public string? PhoneNumber { get; set; }

    public string? DeliveryAddress { get; set; }

    public string? BillingAddress { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
