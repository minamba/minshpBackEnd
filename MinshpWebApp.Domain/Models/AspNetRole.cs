using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class AspNetRole
    {
        public string Id { get; set; } = null!;

        public string? Name { get; set; }

        public string? NormalizedName { get; set; }
    }
}
