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
        Task<ShipmentResultV3> CreateShipmentAsync(CreateShipmentCmd cmd); //pour la v3
        //Task<ShipmentResult> CreateShipmentAsync(CreateShipmentV1Cmd cmd);

        Task<List<RelaysAddressViewModel>> GetRelaysByAddressAsync(RelaysAddressRequest q);

        Task<CodeCategoriesViewModel> GetContentCategoriesAsync();

        Task<Tracking> GetShippingTrackingAsync(string shippingBoxtalReference);

        Task<LiveTracking> CreateSubscriptionAsync(string evenType);
        Task<LiveTracking> GetSubscriptionAsync();
    }
}
