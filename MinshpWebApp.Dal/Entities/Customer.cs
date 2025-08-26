using MinshpConsolApp.Models;
using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class Customer
{
    public int Id { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? PhoneNumber { get; set; }

    public bool Actif { get; set; }

    public string? Civilite { get; set; }

    public string? Email { get; set; }

    public string? IdAspNetUser { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Pseudo { get; set; }

    public long ClientNumberInt { get; set; }

    public string? ClientNumber1 { get; set; }

    public virtual ICollection<BillingAddress> BillingAddresses { get; set; } = new List<BillingAddress>();

    public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; } = new List<DeliveryAddress>();

    public virtual AspNetUser? IdAspNetUserNavigation { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
