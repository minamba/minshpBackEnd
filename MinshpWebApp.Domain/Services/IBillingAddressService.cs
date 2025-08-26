using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface IBillingAddressService
    {
        Task<IEnumerable<BillingAddress>> GetBillingAddressesAsync();
        Task<BillingAddress> UpdateBillingAddressAsync(BillingAddress model);
        Task<BillingAddress> AddBillingAddresssAsync(Domain.Models.BillingAddress model);
        Task<bool> DeleteBillingAddresssAsync(int idBillingAddress);
    }
}
