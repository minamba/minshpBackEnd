using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IFeatureCategoryViewModelBuilder
    {
        Task<IEnumerable<FeatureCategoryViewModel>> GetFeatureCategoriesAsync();
        Task<FeatureCategory> UpdateFeatureCategoryAsync(FeatureCategoryRequest model);
        Task<FeatureCategory> AddFeatureCategoryAsync(FeatureCategoryRequest model);
        Task<bool> DeleteFeatureCategoryAsync(int idFeatureCategory);
        Task<IEnumerable<FeaturesCategoryProductViewModel>> GetFeaturesCategoryProductAsync(int idProduct);
    }
}
