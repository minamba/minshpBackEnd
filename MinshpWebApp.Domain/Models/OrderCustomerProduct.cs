using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class OrderCustomerProduct
    {
        public int Id { get; set; }

        public int? OrderId { get; set; }

        public int? CustomerId { get; set; }

        public int? ProductId { get; set; }

        public int? Quantity { get; set; }

        public decimal? ProductUnitPrice { get; set; }
    }
}
