using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }

        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public string? Password { get; set; }

        public string? PhoneNumber { get; set; }

        public string? DeliveryAddress { get; set; }

        public string? BillingAddress { get; set; }
    }
}
