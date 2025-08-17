using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class Taxe
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? Purcentage { get; set; }

        public decimal? Amount { get; set; }
    }
}
