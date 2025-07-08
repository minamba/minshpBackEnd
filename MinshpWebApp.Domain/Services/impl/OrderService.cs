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
        public Task<Order> AddOrdersAsync(Order model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteOrdersAsync(int idOrder)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Order> UpdateOrdersAsync(Order model)
        {
            throw new NotImplementedException();
        }
    }
}
