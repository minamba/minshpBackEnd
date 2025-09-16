using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class Application
    {
        public int Id { get; set; }

        public int? DisplayNewProductNumber { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }


        public string? DefaultDropOffMondialRelay { get; set; }

        public string? DefaultDropOffChronoPost { get; set; }

        public string? DefaultDropOffUps { get; set; }

        public string? DefaultDropLaposte { get; set; }

        public string? SocietyName { get; set; }
        public string? SocietyAddress { get; set; }
        public string? SocietyZipCode { get; set; }
        public string? SocietyCity { get; set; }
    }
}
