using MinshpWebApp.Domain.Dtos;
using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
        Task<Product> UpdateProductsAsync(Product model);
        Task<Product> AddProductsAsync(Product model);
        Task<bool> DeleteProductsAsync(int idProduct);
        Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<int> ids);
        Task<PageResult<int>> PageProductIdsAsync(PageRequest req, CancellationToken ct = default);
    }
}
