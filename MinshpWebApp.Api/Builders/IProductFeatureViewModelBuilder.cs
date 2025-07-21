using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IProductFeatureViewModelBuilder
    {
        Task<IEnumerable<ProductFeatureViewModel>> GetProductFeaturesAsync();
        Task<ProductFeature> UpdateProductFeaturesAsync(ProductFeatureRequest model);
        Task<ProductFeature> AddProductFeaturesAsync(ProductFeatureRequest model);
        Task<bool> DeleteProductFeaturesAsync(int idProductFeature);
    }
}
