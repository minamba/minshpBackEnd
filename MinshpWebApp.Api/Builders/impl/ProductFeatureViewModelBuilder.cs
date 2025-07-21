using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;

namespace MinshpWebApp.Api.Builders.impl
{
    public class ProductFeatureViewModelBuilder : IProductFeatureViewModelBuilder
    {
        private IMapper _mapper;
        private IProductFeatureService _productFeatureService;


        public ProductFeatureViewModelBuilder(IProductFeatureService productFeatureService, IMapper mapper)
        {
            _mapper = mapper;
            _productFeatureService = productFeatureService;
        }


        public async Task<ProductFeature> AddProductFeaturesAsync(ProductFeatureRequest model)
        {
            return await _productFeatureService.AddProductFeaturesAsync(_mapper.Map<ProductFeature>(model));
        }

        public async Task<bool> DeleteProductFeaturesAsync(int idProductFeature)
        {
            return await _productFeatureService.DeleteProductFeaturesAsync(idProductFeature);
        }

        public async Task<IEnumerable<ProductFeatureViewModel>> GetProductFeaturesAsync()
        {
            var result = await _productFeatureService.GetProductFeaturesAsync();

            return _mapper.Map<IEnumerable<ProductFeatureViewModel>>(result);
        }

        public async Task<ProductFeature> UpdateProductFeaturesAsync(ProductFeatureRequest model)
        {
            var ProductFeature = _mapper.Map<ProductFeature>(model);
            var result = await _productFeatureService.UpdateProductFeaturesAsync(ProductFeature);

            return result;
        }
    }
}
