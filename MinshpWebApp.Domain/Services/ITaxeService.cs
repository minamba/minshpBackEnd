using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface ITaxeService
    {
        Task<IEnumerable<Taxe>> GetTaxesAsync();
        Task<Taxe> UpdateTaxeAsync(Taxe model);
        Task<Taxe> AddTaxeAsync(Domain.Models.Taxe model);
        Task<bool> DeleteTaxeAsync(int idTaxe);
    }
}
