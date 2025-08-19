using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class PromotionCode
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public int? Purcentage { get; set; }

        public DateTime? DateCreation { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsUsed { get; set; }
    }
}
