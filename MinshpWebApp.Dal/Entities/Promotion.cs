using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class Promotion
{
    public int Id { get; set; }

    public int? Purcentage { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? Id_product { get; set; }

    public DateTime? DateCreation { get; set; }

    public virtual Product? IdProductNavigation { get; set; }
}
