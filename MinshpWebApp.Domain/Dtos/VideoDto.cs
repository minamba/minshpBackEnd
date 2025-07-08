using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Dtos
{
    public class VideoDto
    {
        public int Id { get; set; }

        public string? Url { get; set; }

        public string? Description { get; set; }

        public int? IdProduct { get; set; }
    }
}
