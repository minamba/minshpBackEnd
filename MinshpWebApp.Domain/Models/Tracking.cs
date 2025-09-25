using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class Tracking
    {
        public string PackageId { get; set; }
        public string PackageExternalId { get; set; }
        public string TrackingNumber { get; set; }
        public string PackageTrackingUrl { get; set; }
        public DateTime TrackingDateTime { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public bool IsFinal { get; set; }
        public List<History> History { get; set; }
    }

    public class History
    {
        public DateTime TrackingDateTime { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public bool IsFinal { get; set; }
    }
}
