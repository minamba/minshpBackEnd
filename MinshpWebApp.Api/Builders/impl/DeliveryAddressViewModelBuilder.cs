using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;

namespace MinshpWebApp.Api.Builders.impl
{
    public class DeliveryAddressViewModelBuilder : IDeliveryAddressViewModelBuilder
    {
        private IMapper _mapper;
        private IDeliveryAddressService _deliveryAddressService;

        public DeliveryAddressViewModelBuilder(IDeliveryAddressService deliveryAddressService, IMapper mapper)
        {
            _mapper = mapper;
            _deliveryAddressService = deliveryAddressService;
        }

        public async Task<DeliveryAddress> AddDeliveryAddresssAsync(DeliveryAddressRequest model)
        {
            var counter = (await _deliveryAddressService.GetDeliveryAddressesAsync()).Where(p => p.IdCustomer == model.IdCustomer).Count();

            if (counter == 0)
                model.Favorite = true;

            return await _deliveryAddressService.AddDeliveryAddresssAsync(_mapper.Map<DeliveryAddress>(model));
        }

        public async Task<bool> DeleteDeliveryAddresssAsync(int idDeliveryAddress)
        {
            return await _deliveryAddressService.DeleteDeliveryAddresssAsync(idDeliveryAddress);
        }

        public async Task<IEnumerable<DeliveryAddressViewModel>> GetDeliveryAddressesAsync()
        {
            var result = await _deliveryAddressService.GetDeliveryAddressesAsync();

            return _mapper.Map<IEnumerable<DeliveryAddressViewModel>>(result);
        }

        public async Task<DeliveryAddress> UpdateDeliveryAddressAsync(DeliveryAddressRequest model)
        {

            if (model.Favorite == true)
            {
                var deliveryAdressList = await _deliveryAddressService.GetDeliveryAddressesAsync();

                foreach (var d in deliveryAdressList)
                {
                    if (d.Id != model.Id && d.IdCustomer == model.IdCustomer)
                    {
                        d.Favorite = false;
                        await _deliveryAddressService.UpdateDeliveryAddressAsync(_mapper.Map<DeliveryAddress>(d));
                    }
                }
            }

            var _deliveryAddress = _mapper.Map<DeliveryAddress>(model);
            var result = await _deliveryAddressService.UpdateDeliveryAddressAsync(_deliveryAddress);

            return result;
        }
    }
}
