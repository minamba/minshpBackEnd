using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;

namespace MinshpWebApp.Api.Builders.impl
{
    public class FeatureViewModelBuilder : IFeatureViewModelBuilder
    {
        private IMapper _mapper;
        private IFeatureService _featureService;


        public FeatureViewModelBuilder(IFeatureService featureService, IMapper mapper)
        {
            _mapper = mapper;
            _featureService = featureService;
        }

        public async Task<Feature> AddFeaturesAsync(FeatureRequest model)
        {
            var newFeature = _mapper.Map<Feature>(model);

            return await _featureService.AddFeaturesAsync(newFeature);
        }

        public async Task<bool> DeleteFeaturesAsync(int idFeature)
        {
            return await _featureService.DeleteFeaturesAsync(idFeature);
        }

        public async Task<IEnumerable<FeatureViewModel>> GetFeaturesAsync()
        {
            var features = await _featureService.GetFeaturesAsync();
            return _mapper.Map<IEnumerable<FeatureViewModel>>(features);
        }

        public async Task<Feature> UpdateFeaturesAsync(FeatureRequest model)
        {
            var feature = _mapper.Map<Feature>(model);

            return await _featureService.UpdateFeaturesAsync(feature);
        }
    }
}
