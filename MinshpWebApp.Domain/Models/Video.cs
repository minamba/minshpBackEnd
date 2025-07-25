﻿using System;
using System.Collections.Generic;

namespace MinshpWebApp.Domain.Models;

public partial class Video
{
    public int Id { get; set; }

    public string? Url { get; set; }

    public string? Description { get; set; }

    public string? Title { get; set; }

    public int? IdProduct { get; set; }

    public int? Position { get; set; }

}
