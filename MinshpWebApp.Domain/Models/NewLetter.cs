using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class NewLetter
    {
        public int Id { get; set; }

        public string? Mail { get; set; }
        public string? OldMAil { get; set; }

        public bool? Suscribe { get; set; }
    }
}
