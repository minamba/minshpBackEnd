using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface ICustomerRateService
    {
        Task<IEnumerable<CustomerRate>> GetCustomerRatesAsync();
        Task<CustomerRate> UpdateCustomerRateAsync(CustomerRate model);
        Task<CustomerRate> AddCustomerRateAsync(Domain.Models.CustomerRate model);
        Task<bool> DeleteCustomerRateAsync(int idCustomerRate);
    }
}
