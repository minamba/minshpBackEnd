using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class Video
{
    public int Id { get; set; }

    public string? Url { get; set; }

    public string? Description { get; set; }

    public int? Id_product { get; set; }

    public virtual Product? IdProductNavigation { get; set; }
}
