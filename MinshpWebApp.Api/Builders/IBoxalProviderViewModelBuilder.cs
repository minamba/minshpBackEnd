using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services.Shipping;

namespace MinshpWebApp.Api.Builders
{
    public interface IBoxalProviderViewModelBuilder
    {
        Task<List<RateViewModel>> GetRatesAsync(OrderDetailsRequest request);
        Task<List<Relay>> GetRelaysAsync(string zip, string country, int limit = 20);
        Task<ShipmentResult> CreateShipmentAsync(CreateShipmentCmd cmd);

        Task<List<RelaysAddressViewModel>> GetRelaysByAddressAsync(RelaysAddressRequest q);

        Task<CodeCategoriesViewModel> GetContentCategoriesAsync();
    }
}
