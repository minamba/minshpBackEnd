using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IProductViewModelBuilder
    {
        Task<IEnumerable<ProductVIewModel>> GetProductsAsync();
        Task<Product> AddProductsAsync(ProductRequest model);
        Task<Product> UpdateProductsAsync(ProductRequest model);
        Task<bool> DeleteProductsAsync(int idProduct);
    }
}
