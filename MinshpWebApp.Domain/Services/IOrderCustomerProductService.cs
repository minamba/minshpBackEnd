using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface IOrderCustomerProductService
    {
        Task<IEnumerable<OrderCustomerProduct>> GetOrderCustomerProductsAsync();
        Task<OrderCustomerProduct> UpdateOrderCustomerProductsAsync(OrderCustomerProduct model);
        Task<OrderCustomerProduct> AddOrderCustomerProductsAsync(Domain.Models.OrderCustomerProduct model);
        Task<bool> DeleteOrderCustomerProductsAsync(int idOrderCustomerProduct);
    }
}
