﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public int? Stock { get; set; }
        public int? IdCategory { get; set; }

        public bool? Main { get; set; }
    }
}
