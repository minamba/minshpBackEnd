using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;

namespace MinshpWebApp.Api.Builders.impl
{
    public class CustomerRateViewModelBuilder : ICustomerRateViewModelBuilder
    {

        private IMapper _mapper;
        private ICustomerRateService _CustomerRateService;
        private ICustomerViewModelBuilder _CustomerViewModelBuilder;
        private IProductViewModelBuilder _ProductViewModelBuilder;


        public CustomerRateViewModelBuilder(ICustomerRateService CustomerRateService, ICustomerViewModelBuilder CustomerViewModelBuilder, IProductViewModelBuilder ProductViewModelBuilder, IMapper mapper)
        {
            _mapper = mapper;
            _CustomerRateService = CustomerRateService;
            _CustomerViewModelBuilder = CustomerViewModelBuilder;
            _ProductViewModelBuilder = ProductViewModelBuilder;
        }

        public async Task<CustomerRate> AddCustomerRateAsync(CustomerRateRequest model)
        {
            var newCustomerRate = _mapper.Map<CustomerRate>(model);
            return await _CustomerRateService.AddCustomerRateAsync(newCustomerRate);
        }


        public async Task<bool> DeleteCustomerRateAsync(int idCustomerRate)
        {
            return await _CustomerRateService.DeleteCustomerRateAsync(idCustomerRate);
        }


        public async Task<IEnumerable<CustomerRateViewModel>> GetCustomerRatesAsync()
        {
            var CustomerRates = await _CustomerRateService.GetCustomerRatesAsync();
            var customers = await _CustomerViewModelBuilder.GetCustomersAsync();
            var products =  await _ProductViewModelBuilder.GetProductsAsync();

            var result = _mapper.Map<IEnumerable<CustomerRateViewModel>>(CustomerRates);

            foreach(var r in result)
            {
                r.customer = customers.FirstOrDefault(c => c.Id == r.IdCustomer);
                r.product = products.FirstOrDefault(p => p.Id == r.IdProduct);
            }

            return result;
        }

        public async Task<CustomerRate> UpdateCustomerRateAsync(CustomerRateRequest model)
        {
            var CustomerRate = _mapper.Map<CustomerRate>(model);

            return await _CustomerRateService.UpdateCustomerRateAsync(CustomerRate);
        }
    }
}
