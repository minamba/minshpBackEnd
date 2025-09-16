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
        private IDeliveryAddressViewModelBuilder _deliveryAddressService;
        private IBillingAddressViewModelBuilder _billingAddressService;
        private IMailViewModelBuilder _mailViewModelBuilder;


        public CustomerViewModelBuilder(ICustomerService customerService, IDeliveryAddressViewModelBuilder deliveryAddressService, IBillingAddressViewModelBuilder billingAddressService, IMailViewModelBuilder mailViewModelBuilder, IMapper mapper)
        {
            _mapper = mapper;
            _customerService = customerService;
            _deliveryAddressService = deliveryAddressService;
            _billingAddressService = billingAddressService;
            _mailViewModelBuilder = mailViewModelBuilder;
        }

        public async Task<Customer> AddCustomersAsync(CustomerRequest model)
        {
            var customer = _mapper.Map<Customer>(model);

            var result = await _customerService.AddCustomersAsync(customer);


            if (result != null)
                _mailViewModelBuilder.SendMailRegistration("minamba.c@gmail.com");

            return result;
        }

        public async Task<bool> DeleteCustomersAsync(int idCustomer)
        {
            return await _customerService.DeleteCustomersAsync(idCustomer);
        }

        public async Task<IEnumerable<CustomerViewModel>> GetCustomersAsync()
        {
            var result = await _customerService.GetCustomersAsync();



            var list =  _mapper.Map<IEnumerable<CustomerViewModel>>(result);


            foreach (var item in list)
            {
                var getDeliveryAdresses = (await _deliveryAddressService.GetDeliveryAddressesAsync()).Where(p => p.IdCustomer == item.Id);
                var getBillingAdresse = (await _billingAddressService.GetBillingAddressesAsync()).FirstOrDefault(p => p.IdCustomer == item.Id);
                item.DeliveryAddresses = getDeliveryAdresses;
                item.BillingAddress = getBillingAdresse;
            }

            return list;

        }

        public async Task<Customer> UpdateCustomersAsync(CustomerRequest model)
        {
            var customer = _mapper.Map<Customer>(model);
            return await _customerService.UpdateCustomersAsync(customer);
        }
    }
}
