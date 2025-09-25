using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;

namespace MinshpWebApp.Api.Builders.impl
{
    public class CustomerPromotionCodeViewModelBuilder : ICustomerPromotionCodeViewModelBuilder
    {
        private IMapper _mapper;
        private ICustomerPromotionCodeService _CustomerPromotionCodeService;


        public CustomerPromotionCodeViewModelBuilder(ICustomerPromotionCodeService CustomerPromotionCodeService, IMapper mapper)
        {
            _mapper = mapper;
            _CustomerPromotionCodeService = CustomerPromotionCodeService;
        }


        public async Task<CustomerPromotionCode> AddCustomerPromotionCodesAsync(CustomerPromotionCodeRequest model)
        {
            return await _CustomerPromotionCodeService.AddCustomerPromotionCodesAsync(_mapper.Map<CustomerPromotionCode>(model));
        }

        public async Task<bool> DeleteCustomerPromotionCodesAsync(int idCustomerPromotionCode)
        {
            return await _CustomerPromotionCodeService.DeleteCustomerPromotionCodesAsync(idCustomerPromotionCode);
        }

        public async Task<IEnumerable<CustomerPromotionCodeViewModel>> GetCustomerPromotionCodesAsync()
        {
            var result = await _CustomerPromotionCodeService.GetCustomerPromotionCodesAsync();

            return _mapper.Map<IEnumerable<CustomerPromotionCodeViewModel>>(result);
        }

        public async Task<CustomerPromotionCode> UpdateCustomerPromotionCodesAsync(CustomerPromotionCodeRequest model)
        {
            var CustomerPromotionCode = _mapper.Map<CustomerPromotionCode>(model);
            var result = await _CustomerPromotionCodeService.UpdateCustomerPromotionCodesAsync(CustomerPromotionCode);

            return result;
        }
    }
}
