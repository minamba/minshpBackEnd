using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using Customer = MinshpWebApp.Domain.Models.Customer;

namespace MinshpWebApp.Api.Builders.impl
{
    public class CustomerViewModelBuilder : ICustomerViewModelBuilder
    {

        private IMapper _mapper;
        private ICustomerService _customerService;


        public CustomerViewModelBuilder(ICustomerService customerService, IMapper mapper)
        {
            _mapper = mapper;
            _customerService = customerService;
        }

        public async Task<Customer> AddCustomersAsync(CustomerRequest model)
        {
            var customer = _mapper.Map<Customer>(model);

            return await _customerService.AddCustomersAsync(customer);
        }

        public async Task<bool> DeleteCustomersAsync(int idCustomer)
        {
            return await _customerService.DeleteCustomersAsync(idCustomer);
        }

        public async Task<IEnumerable<CustomerViewModel>> GetCustomersAsync()
        {
            var result = await _customerService.GetCustomersAsync();

            return _mapper.Map<IEnumerable<CustomerViewModel>>(result);
        }

        public async Task<Customer> UpdateCustomersAsync(CustomerRequest model)
        {
            var customer = _mapper.Map<Customer>(model);
            return await _customerService.UpdateCustomersAsync(customer);
        }
    }
}
