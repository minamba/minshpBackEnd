using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;
public partial class NewLetter
{
    public int Id { get; set; }

    public string? Mail { get; set; }

    public bool? Suscribe { get; set; }
}
