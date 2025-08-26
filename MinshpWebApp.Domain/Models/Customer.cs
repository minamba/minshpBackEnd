using System;
using System.Collections.Generic;

namespace MinshpWebApp.Domain.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? PhoneNumber { get; set; }

    public bool Actif { get; set; }

    public string? Civilite { get; set; }

    public string? ClientNumber { get; set; }

    public string? Email { get; set; }

    public string? IdAspNetUser { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Pseudo { get; set; }

    public string? ClientNumber1 { get; set; }
}
