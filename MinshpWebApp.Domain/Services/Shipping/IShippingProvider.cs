using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services.Shipping.impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MinshpWebApp.Domain.Services.Shipping
{
    public record OpeningInterval(string openingTime, string closingTime);

    // Ajout d'un dernier paramètre optionnel: openingDays
    public record Relay(
        string id,
        string name,
        string address,
        string zip,
        string city,
        double lat,
        double lng,
        int distance,
        string network,
        Dictionary<string, List<OpeningInterval>>? openingDays = null
    );

    //public record CreateShipmentCmd(
    //    string serviceCode, bool isRelay, string? relayId,
    //    string toFirstName, string toLastName,
    //    string toStreet, string? toExtra,
    //    string toZip, string toCity, string toCountry,
    //    decimal weightKg, decimal declaredValue);

    public record ShipmentResult(string providerShipmentId, string carrier, string serviceCode, string trackingNumber, string labelUrl);

    public interface IShippingProvider
    {
        Task<List<Rate>> GetRatesAsync(OrderDetails orderDetails);
        Task<List<Relay>> GetRelaysAsync(string zip, string country, int limit = 20);
        Task<ShipmentResultV3> CreateShipmentAsync(CreateShipmentCmd cmd); //pour la v3
        //Task<ShipmentResult> CreateShipmentAsync(CreateShipmentV1Cmd cmd); //pour la v3
        Task<List<Relay>> GetRelaysByAddressAsync(RelaysAddress q);
        Task<CodeCategories> GetContentCategoriesAsync();

        Task<Tracking> GetShippingTrackingAsync(string shippingBoxtalReference);

        Task<LiveTracking> CreateSubscriptionAsync(string eventType);
        Task<LiveTracking> GetSubscriptionAsync();
    }
}
