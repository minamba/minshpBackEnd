using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public int? OrderId { get; set; }

        public int? CustomerId { get; set; }

        public DateTime? DateCreation { get; set; }

        public string? Representative { get; set; }

        public string? InvoiceNumber { get; set; }

        public string? InvoiceLink { get; set; }

        public bool? HardDelete { get; set; } = false;

    }
}
