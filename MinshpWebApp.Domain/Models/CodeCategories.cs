using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{

    public class CodeCategory
    {
        public string Id { get; set; }
        public string Label { get; set; }
    }

    public class CodeCategories
    {
        public List<CodeCategory> AllCodeCategories { get; set; } = new List<CodeCategory>();
    }
}
