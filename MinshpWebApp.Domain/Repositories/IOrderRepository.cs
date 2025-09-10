using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> UpdateOrdersAsync(Order model);
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<Order> AddOrdersAsync(Domain.Models.Order model);
        Task<bool> DeleteOrdersAsync(int idOrder);

        Task<Order> FindByShipmentIdAsync(string providerShipmentId);
        Task<Order> GetByIdAsync(string id);
    }
}
