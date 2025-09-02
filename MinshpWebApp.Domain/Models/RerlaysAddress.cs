using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public sealed class RelaysAddress
    {
        public string? Number { get; set; }         // ex: "4"
        public string? Street { get; set; }         // "boulevard des capucines"
        public string? City { get; set; }           // "Paris"
        public string? PostalCode { get; set; }     // "75009"
        public string? State { get; set; }          // (optionnel)
        public string CountryIsoCode { get; set; } = "FR"; // ⚠ requis (2 lettres)
        public IEnumerable<string>? SearchNetworks { get; set; } // ex: ["MONR","CHRP"]
        public int Limit { get; set; } = 30;        // Boxtal accepte "limit" sur v3.1
    }
}
