using AutoMapper;
using Azure.Core;
using Microsoft.Build.Tasks;
using MinshpWebApp.Api.Enums;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.Utils;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services.Shipping;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;

namespace MinshpWebApp.Api.Builders.impl
{
    public class BoxalProviderViewModelBuilder : IBoxalProviderViewModelBuilder
    {
        const string monday = "lundi";
        const string tuesday = "mardi";
        const string wednesday = "mercredi";
        const string thursday = "jeudi";
        const string friday = "vendredi";
        const string saturday = "samedi";
        const string sunday = "dimanche";

        const string chronoPost = "Chronopost";
        const string mondialRelay = "Mondial relay";
        const string ups = "Ups";

        private IMapper _mapper;
        IShippingProvider _shippingProvider;

        public BoxalProviderViewModelBuilder(IShippingProvider shippingProvider, IMapper mapper)
        {
            _shippingProvider = shippingProvider;
            _mapper = mapper;
        }

        //pour la v3
        public async Task<ShipmentResultV3> CreateShipmentAsync(CreateShipmentCmd cmd)
        {
            return await _shippingProvider.CreateShipmentAsync(cmd);
        }

        //public async Task<ShipmentResult> CreateShipmentAsync(CreateShipmentV1Cmd cmd)
        //{
        //    return await _shippingProvider.CreateShipmentAsync(cmd);
        //}

        public async Task<List<RateViewModel>> GetRatesAsync(OrderDetailsRequest request)
        {
            var orderDetails = _mapper.Map<OrderDetails>(request);
            var rates = await _shippingProvider.GetRatesAsync(orderDetails);

            foreach (var r in rates)
            {
                if (r.PriceTtc > 50)
                    r.PriceTtc = r.PriceTtc - (r.PriceTtc * (15/100m));
            }



            return _mapper.Map<List<RateViewModel>>(rates);

        }

        public async Task<List<Relay>> GetRelaysAsync(string zip, string country, int limit = 20)
        {
           return await _shippingProvider.GetRelaysAsync(zip, country, limit);
        }

        public async Task<List<RelaysAddressViewModel>> GetRelaysByAddressAsync(RelaysAddressRequest q)
        {
            var newRelaysByAddress = _mapper.Map<RelaysAddress>(q);
            var relayLst =  await _shippingProvider.GetRelaysByAddressAsync(newRelaysByAddress);
            var relayAddressLstVm = new List<RelaysAddressViewModel>();

            var address = q.Number + " " + q.Street + " " + q.PostalCode + " " + q.City;

            var getGeoPointFromAddress = await Geo.GeocodeWithNominatimAsync(address);


            if (getGeoPointFromAddress != null)
            {
                foreach (var r in relayLst)
                {



                    if (Enum.IsDefined(typeof(CarrierEnum), r.network))
                    {
                        var relayAddressVm = new RelaysAddressViewModel()
                        {
                            Id = r.id,
                            City = r.city,
                            Name = r.name,
                            Network = r.network,
                            Carrier = await GetCarrier(r.network),
                            Address = r.address,
                            Distance = await GetDistance(r.distance.ToString()),  //await GetDistance(getGeoPointFromAddress.Lat, getGeoPointFromAddress.Lon, r.lat, r.lng),
                            Latitude = r.lat,
                            Longitude = r.lng,
                            ZipCode = r.zip,
                            Schedules = await GetSchedules(r.openingDays)
                        };

                        relayAddressLstVm.Add(relayAddressVm);
                    }
                }

                var result = relayAddressLstVm
                            .OrderBy(p => ParseMeters(p.Distance))
                            .ToList();

                return result;
            }

            return null;
        }


        public async Task<CodeCategoriesViewModel> GetContentCategoriesAsync()
        {
            var codeCategories = await _shippingProvider.GetContentCategoriesAsync();

            var result =  _mapper.Map<CodeCategoriesViewModel>(codeCategories);

            foreach (var r in result.AllCodeCategories)
            {
                var code = r.Id.Split(":");
                r.Id = code[2];
            }

            return result;
        }



        public async Task<Tracking> GetShippingTrackingAsync(string shippingBoxtalReference)
        {
            return await _shippingProvider.GetShippingTrackingAsync(shippingBoxtalReference);
        }


        public async Task<LiveTracking> CreateSubscriptionAsync(string evenType)
        {
            return await _shippingProvider.CreateSubscriptionAsync(evenType);
        }

        public async Task<LiveTracking> GetSubscriptionAsync()
        {
            return await _shippingProvider.GetSubscriptionAsync();
        }



        // HELPERS ****************************************************************************************************************

        //METHODE POUR LA V3
        private async Task<string?> GetSchedules(Dictionary<string, List<OpeningInterval>> OpeningDays)
        {
            int countOpening = 0;
            int countClosing = 0;

            string openDay = null;
            string closeDay = null;

            string openHour = null;
            string closeHour = null;

            string schedules = null;

            foreach (var o in OpeningDays.ToList())
            {
                DaysEnum dayEnum = Enum.Parse<DaysEnum>(o.Key);

                string day = dayEnum switch
                {
                    DaysEnum.MONDAY => monday,
                    DaysEnum.TUESDAY => tuesday,
                    DaysEnum.WEDNESDAY => wednesday,
                    DaysEnum.THURSDAY => thursday,
                    DaysEnum.FRIDAY => friday,
                    DaysEnum.SATURDAY => saturday,
                    DaysEnum.SUNDAY => sunday,
                };

                foreach (var s in o.Value)
                {
                    if (s.openingTime != null)
                    {
                        countOpening++;
                        if (countOpening == 1)
                        {
                            openDay = day;
                            openHour = s.openingTime.ToString();
                        }
                    }


                    if (s.closingTime != null)
                    {
                        countClosing++;
                        closeHour = s.closingTime.ToString();
                    }

                    closeDay = day;
                }



            }
            schedules = openDay + "-" + closeDay + " : " + openHour + "-" + closeHour;
            return schedules;

        }



        //METHODE POUR LA V1
        //private async Task<string?> GetSchedules(Dictionary<string, List<OpeningInterval>> OpeningDays)
        //{
        //    int countOpening = 0;
        //    int countClosing = 0;

        //    string openDay = null;
        //    string closeDay = null;

        //    string openHour = null;
        //    string closeHour = null;

        //    string schedules = null;

        //    foreach (var o in OpeningDays.ToList())
        //    {
        //        DaysEnum dayEnum = Enum.Parse<DaysEnum>(o.Key.ToUpper());

        //        string day = dayEnum switch
        //        {
        //            DaysEnum.MONDAY => monday,
        //            DaysEnum.TUESDAY => tuesday,
        //            DaysEnum.WEDNESDAY => wednesday,
        //            DaysEnum.THURSDAY => thursday,
        //            DaysEnum.FRIDAY => friday,
        //            DaysEnum.SATURDAY => saturday,
        //            DaysEnum.SUNDAY => sunday,
        //        };

        //        foreach (var s in o.Value)
        //        {
        //            if (s.openingTime != null)
        //            {
        //                countOpening++;
        //                if (countOpening == 1)
        //                {
        //                    openDay = day;
        //                    openHour = s.openingTime.ToString();
        //                }
        //            }


        //            if (s.closingTime != null)
        //            {
        //                countClosing++;
        //                closeHour = s.closingTime.ToString();
        //            }

        //            closeDay = day;
        //        }



        //    }
        //    schedules = openDay + "-" + closeDay + " : " + openHour + "-" + closeHour;
        //    return schedules;

        //}


        private async Task<string> GetCarrier(string network)
        {
            CarrierEnum carrierEnum = Enum.Parse<CarrierEnum>(network);

            string carrier = carrierEnum switch
            {
                CarrierEnum.MONR => mondialRelay,
                CarrierEnum.UPSE => ups,
                CarrierEnum.CHRP => chronoPost,
                _ => network
            };

            return carrier;

        }


        // GET DISTANCE POUR LA V3
        private async Task<string> GetDistance(string distance)
        {
            double d = int.Parse(distance);
            string dist = null;


            if (d < 100)
            {
                d = d * 10;
                dist = d.ToString() + "m";
            }
            else if (d >= 1000)
            {
                d = d / 1000;
                dist = d.ToString() + "km";
            }
            else
                dist = d.ToString() + "m";

            return dist;
        }


        // GET DISTANCE POUR LA V1
        //private async Task<string> GetDistance(double latitudeInputAdress, double longitudeinputAdress, double latitudeRelay, double longitudeRelay)
        //{

        //    var distance = Geo.HaversineMeters(latitudeInputAdress, longitudeinputAdress, latitudeRelay, longitudeRelay);

        //    if (distance < 1000)
        //        return $"{distance}m";

        //    var km = distance / 1000.0;
        //    return km < 10
        //        ? $"{km:0.0}km"
        //        : $"{Math.Round(km):0}km";
        //}


        int ParseMeters(string s)
        {
            s = s.ToLowerInvariant().Trim();
            return s.EndsWith("km")
                ? (int)Math.Round(double.Parse(s[..^2], CultureInfo.InvariantCulture) * 1000)
                : int.Parse(s[..^1], CultureInfo.InvariantCulture); // "123m"
        }


    }
}
