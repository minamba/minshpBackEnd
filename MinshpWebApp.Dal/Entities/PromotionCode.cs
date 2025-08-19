using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class PromotionCode
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Purcentage { get; set; }

    public DateTime? DateCreation { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsUsed { get; set; }
}
