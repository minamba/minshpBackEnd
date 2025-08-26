using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IDeliveryAddressViewModelBuilder
    {
        Task<IEnumerable<DeliveryAddressViewModel>> GetDeliveryAddressesAsync();
        Task<DeliveryAddress> UpdateDeliveryAddressAsync(DeliveryAddressRequest model);
        Task<DeliveryAddress> AddDeliveryAddresssAsync(DeliveryAddressRequest model);
        Task<bool> DeleteDeliveryAddresssAsync(int idDeliveryAddress);
    }
}
