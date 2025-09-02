using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class Rate
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public string Carrier { get; set; }
        public decimal PriceTtc { get; set; }
        public bool IsRelay { get; set; }
    }
}
