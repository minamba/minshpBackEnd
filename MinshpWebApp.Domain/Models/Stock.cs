﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class Stock
    {
        public int Id { get; set; }

        public int? Quantity { get; set; }

        public int? IdProduct { get; set; }

    }
}
