using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class CustomerRate
    {
        public int? Id { get; set; }

        public int? IdCustomer { get; set; }

        public int? IdProduct { get; set; }

        public int? Rate { get; set; }

        public string? Title { get; set; }

        public string? Message { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? ModificationDate { get; set; }
    }
}
