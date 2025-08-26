using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IBillingAddressViewModelBuilder
    {
        Task<IEnumerable<BillingAddressViewModel>> GetBillingAddressesAsync();
        Task<BillingAddress> UpdateBillingAddressAsync(BillingAddressRequest model);
        Task<BillingAddress> AddBillingAddresssAsync(BillingAddressRequest model);
        Task<bool> DeleteBillingAddresssAsync(int idBillingAddress);
    }
}
