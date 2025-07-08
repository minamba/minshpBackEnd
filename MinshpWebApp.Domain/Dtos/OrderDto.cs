using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }

        public Guid? OrderNumber { get; set; }

        public int? Quantity { get; set; }

        public DateTime? Date { get; set; }

        public string? Status { get; set; }

        public int? IdCustomer { get; set; }

        public int? IdProduct { get; set; }
    }
}
