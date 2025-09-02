using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class OrderCustomerProductService : IOrderCustomerProductService
    {
        IOrderCustomerProductRepository _repository;

        public OrderCustomerProductService(IOrderCustomerProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrderCustomerProduct> AddOrderCustomerProductsAsync(OrderCustomerProduct model)
        {
            return await _repository.AddOrderCustomerProductsAsync(model);
        }

        public async Task<bool> DeleteOrderCustomerProductsAsync(int idOrderCustomerProduct)
        {
            return await _repository.DeleteOrderCustomerProductsAsync(idOrderCustomerProduct);
        }

        public async Task<IEnumerable<OrderCustomerProduct>> GetOrderCustomerProductsAsync()
        {
            return await _repository.GetOrderCustomerProductsAsync();
        }

        public async Task<OrderCustomerProduct> UpdateOrderCustomerProductsAsync(OrderCustomerProduct model)
        {
            return await _repository.UpdateOrderCustomerProductsAsync(model);
        }
    }
}
