using System;
using System.Collections.Generic;

namespace MinshpWebApp.Domain.Models;

public partial class Customer
{
    public int Id { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public string? Password { get; set; }

    public string? PhoneNumber { get; set; }
}
