using MinshpWebApp.Api.ViewModels;

namespace MinshpWebApp.Api.Builders
{
    public interface IProductViewModelBuilder
    {
        Task<IEnumerable<ProductVIewModel>> GetProductsAsync();
    }
}
