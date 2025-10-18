using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class CustomerRateService : ICustomerRateService
    {

        ICustomerRateRepository _repository;


        public CustomerRateService(ICustomerRateRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerRate> AddCustomerRateAsync(CustomerRate model)
        {
           return await _repository.AddCustomerRateAsync(model);
        }

        public async Task<bool> DeleteCustomerRateAsync(int idCustomerRate)
        {
            return await _repository.DeleteCustomerRateAsync(idCustomerRate);
        }

        public async Task<IEnumerable<CustomerRate>> GetCustomerRatesAsync()
        {
           return await _repository.GetCustomerRatesAsync();
        }

        public async Task<CustomerRate> UpdateCustomerRateAsync(CustomerRate model)
        {
            return await _repository.UpdateCustomerRateAsync(model);
        }
    }
}
