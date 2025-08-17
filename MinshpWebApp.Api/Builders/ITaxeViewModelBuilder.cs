using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface ITaxeViewModelBuilder
    {
        Task<IEnumerable<TaxeViewModel>> GetTaxesAsync();
        Task<Taxe> UpdateTaxeAsync(TaxeRequest model);
        Task<Taxe> AddTaxeAsync(TaxeRequest model);
        Task<bool> DeleteTaxeAsync(int idTaxe);
    }
}
