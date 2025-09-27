using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomersAsync();
        Task<Customer> UpdateCustomersAsync(Customer model);
        Task<Customer> AddCustomersAsync(Domain.Models.Customer model);
        Task<bool> DeleteCustomersAsync(int idCustomer);

        Task<IEnumerable<Customer>> GetCustomersByIdsAsync(IEnumerable<int> ids);
        Task<PageResult<int>> PageCustomerIdsAsync(PageRequest req, CancellationToken ct = default);
    }
}
