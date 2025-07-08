using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class CustomerService : ICustomerService
    {
        ICustomerRepository _repository;


        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }


        public async Task<Customer> AddCustomersAsync(Customer model)
        {
           return await _repository.AddCustomersAsync(model);
        }

        public async Task<bool> DeleteCustomersAsync(int idCustomer)
        {
            return await _repository.DeleteCustomersAsync(idCustomer);
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            return await _repository.GetCustomersAsync();
        }

        public async Task<Customer> UpdateCustomersAsync(Customer model)
        {
            return await _repository.UpdateCustomersAsync(model);
        }
    }
}
