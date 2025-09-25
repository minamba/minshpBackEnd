using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class CustomerPromotionCode
    {
        public int? Id { get; set; }

        public int? IdCutomer { get; set; }

        public int? IdPromotion { get; set; }

        public bool? IsUsed { get; set; }
    }
}
