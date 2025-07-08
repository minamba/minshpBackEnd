using MinshpWebApp.Domain.Dtos;
using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface IOrderService
    {
        Task<Order> UpdateOrdersAsync(Order model);
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<Order> AddOrdersAsync(Domain.Models.Order model);
        Task<bool> DeleteOrdersAsync(int idOrder);
    }
}
