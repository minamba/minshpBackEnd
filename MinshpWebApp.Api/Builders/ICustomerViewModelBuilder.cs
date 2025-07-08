using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface ICustomerViewModelBuilder
    {
        Task<IEnumerable<CustomerViewModel>> GetCustomersAsync();
        Task<Customer> UpdateCustomersAsync(CustomerRequest model);
        Task<Customer> AddCustomersAsync(CustomerRequest model);
        Task<bool> DeleteCustomersAsync(int idCustomer);
    }
}
