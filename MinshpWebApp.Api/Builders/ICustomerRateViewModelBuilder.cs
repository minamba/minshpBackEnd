using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface ICustomerRateViewModelBuilder
    {
        Task<IEnumerable<CustomerRateViewModel>> GetCustomerRatesAsync();
        Task<CustomerRate> UpdateCustomerRateAsync(CustomerRateRequest model);
        Task<CustomerRate> AddCustomerRateAsync(CustomerRateRequest model);
        Task<bool> DeleteCustomerRateAsync(int idCustomerRate);
    }
}
