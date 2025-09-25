using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class CustomerPromotionCode
{
    public int Id { get; set; }

    public int? IdCutomer { get; set; }

    public int? IdPromotion { get; set; }

    public bool? IsUsed { get; set; }

    public virtual Customer? IdCutomerNavigation { get; set; }

    public virtual PromotionCode? IdPromotionNavigation { get; set; }
}
