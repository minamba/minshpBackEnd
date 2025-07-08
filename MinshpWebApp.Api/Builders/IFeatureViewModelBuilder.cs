using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IFeatureViewModelBuilder
    {
        Task<IEnumerable<FeatureViewModel>> GetFeaturesAsync();
        Task<Feature> UpdateFeaturesAsync(FeatureRequest model);
        Task<Feature> AddFeaturesAsync(FeatureRequest model);
        Task<bool> DeleteFeaturesAsync(int idFeature);
    }
}
