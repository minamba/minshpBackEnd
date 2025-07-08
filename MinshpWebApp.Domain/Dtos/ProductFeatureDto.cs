using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Dtos
{
    public class ProductFeatureDto
    {
        public int Id { get; set; }

        public int? IdProduct { get; set; }

        public int? IdFeature { get; set; }

    }
}
