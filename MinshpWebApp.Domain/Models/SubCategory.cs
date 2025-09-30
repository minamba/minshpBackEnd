using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class SubCategory
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? IdPromotionCode { get; set; }

        public int? IdCategory { get; set; }

        public string? IdTaxe { get; set; }

        public int? ContentCode { get; set; }

        public bool? Display { get; set; }
    }
}
