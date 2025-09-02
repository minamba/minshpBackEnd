using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class OrderService : IOrderService
    {
        IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }
        public async Task<Order> AddOrdersAsync(Order model)
        {
            return await _repository.AddOrdersAsync(model);
        }

        public async Task<bool> DeleteOrdersAsync(int idOrder)
        {
           return await _repository.DeleteOrdersAsync(idOrder);
        }

        public async Task<Order> FindByShipmentIdAsync(string providerShipmentId)
        {
            return await _repository.FindByShipmentIdAsync(providerShipmentId);
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await (_repository.GetByIdAsync(id));
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _repository.GetOrdersAsync();
        }

        public async Task<Order> UpdateOrdersAsync(Order model)
        {
           return await _repository.UpdateOrdersAsync(model);
        }
    }
}
