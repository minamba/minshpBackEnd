using MinshpWebApp.Domain.Dtos;
using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetCustomersAsync();
        Task<Customer> UpdateCustomersAsync(Customer model);
        Task<Customer> AddCustomersAsync(Domain.Models.Customer model);
        Task<bool> DeleteCustomersAsync(int idCustomer);
    }
}
