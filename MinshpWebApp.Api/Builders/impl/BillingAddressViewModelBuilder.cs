using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.Utils;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using MinshpWebApp.Domain.Services.impl;

namespace MinshpWebApp.Api.Builders.impl
{
    public class BillingAddressViewModelBuilder : IBillingAddressViewModelBuilder
    {
        private IMapper _mapper;
        private IBillingAddressService _billingAddressService;

        public BillingAddressViewModelBuilder(IBillingAddressService billingAddressService, IMapper mapper)
        {
            _mapper = mapper;
            _billingAddressService = billingAddressService;
        }

        public async Task<BillingAddress> AddBillingAddresssAsync(BillingAddressRequest model)
        {
            return await _billingAddressService.AddBillingAddresssAsync(_mapper.Map<BillingAddress>(model));
        }

        public async Task<bool> DeleteBillingAddresssAsync(int idBillingAddress)
        {
            return await _billingAddressService.DeleteBillingAddresssAsync(idBillingAddress);
        }

        public async Task<IEnumerable<BillingAddressViewModel>> GetBillingAddressesAsync()
        {
            var result = await _billingAddressService.GetBillingAddressesAsync();

            return _mapper.Map<IEnumerable<BillingAddressViewModel>>(result);
        }

        public async Task<BillingAddress> UpdateBillingAddressAsync(BillingAddressRequest model)
        {
            var _billingAddress = _mapper.Map<BillingAddress>(model);
            var result = await _billingAddressService.UpdateBillingAddressAsync(_billingAddress);

            return result;
        }
    }
}
