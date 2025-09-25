using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface ICustomerPromotionCodeService
    {
        Task<IEnumerable<CustomerPromotionCode>> GetCustomerPromotionCodesAsync();
        Task<CustomerPromotionCode> UpdateCustomerPromotionCodesAsync(CustomerPromotionCode model);
        Task<CustomerPromotionCode> AddCustomerPromotionCodesAsync(Domain.Models.CustomerPromotionCode model);
        Task<bool> DeleteCustomerPromotionCodesAsync(int idCustomerPromotionCode);
    }
}
