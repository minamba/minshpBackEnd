using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> UpdateProductsAsync(Product model);
        Task<Product> AddProductsAsync(Product model);
        Task<bool> DeleteProductsAsync(int idProduct);

    }
}
