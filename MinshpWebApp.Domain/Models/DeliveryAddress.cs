using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class DeliveryAddress
    {
        public int Id { get; set; }

        public string? Address { get; set; }

        public string? ComplementaryAddress { get; set; }

        public int? PostalCode { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public bool? Favorite { get; set; }

        public int? IdCustomer { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }

        public string? Civilite { get; set; }
    }
}
