using AutoMapper;
using Azure.Core;
using MinshpWebApp.Api.Enums;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services.Shipping;

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
        public async Task<ShipmentResult> CreateShipmentAsync(CreateShipmentCmd cmd)
        {
           return await _shippingProvider.CreateShipmentAsync(cmd);
        }

        public async Task<List<RateViewModel>> GetRatesAsync(OrderDetailsRequest request)
        {
            var orderDetails = _mapper.Map<OrderDetails>(request);
            var result = await _shippingProvider.GetRatesAsync(orderDetails);

            return _mapper.Map<List<RateViewModel>>(result);
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
                        Distance = await GetDistance(r.distance.ToString()),
                        Latitude = r.lat,
                        Longitude = r.lng,
                        ZipCode = r.zip,
                        Schedules = await GetSchedules(r.openingDays)
                    };

                        relayAddressLstVm.Add(relayAddressVm);
                }
            }


            return relayAddressLstVm;
        }


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

                foreach(var s in o.Value)
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

        private async Task<string> GetDistance (string distance)
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
    }
}
