using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IOrderCustomerProductViewModelBuilder
    {
        Task<IEnumerable<OrderCustomerProductViewModel>> GetOrderCustomerProductsAsync();
        Task<OrderCustomerProduct> UpdateOrderCustomerProductsAsync(OrderCustomerProductRequest model);
        Task<OrderCustomerProduct> AddOrderCustomerProductsAsync(OrderCustomerProductRequest model);
        Task<bool> DeleteOrderCustomerProductsAsync(OrderCustomerProductRequest model);
    }
}
