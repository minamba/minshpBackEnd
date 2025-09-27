using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IOrderViewModelBuilder
    {
        Task<Order> UpdateOrdersAsync(OrderRequest model);
        Task<IEnumerable<OrderViewModel>> GetOrdersAsync();
        Task<Order> AddOrdersAsync(OrderRequest model);
        Task<bool> DeleteOrdersAsync(int idOrder);

        Task<Order> FindByShipmentIdAsync(string providerShipmentId);
        Task<Order> GetByIdAsync(string id);

        Task<IEnumerable<Order>> GetOrdersByIdsAsync(IEnumerable<int> ids);
        Task<PageResult<OrderViewModel>> PageOrderIdsAsync(PageRequest req, CancellationToken ct = default);
    }
}
