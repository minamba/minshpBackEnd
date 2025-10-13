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
        private IFeatureService _featureService;
        private ICategoryService _categoryService;
        private IProductService _productService;

        public ProductFeatureViewModelBuilder(IProductFeatureService productFeatureService, IProductService productService, ICategoryService categoryService, IFeatureService featureService, IMapper mapper)
        {
            _mapper = mapper;
            _productFeatureService = productFeatureService;
            _featureService = featureService;
            _categoryService = categoryService;
            _productService = productService;
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
            var products = await _productService.GetProductsAsync();
            var categories = await _categoryService.GetCategoriesAsync();
            var features = await _featureService.GetFeaturesAsync();

            var result = await _productFeatureService.GetProductFeaturesAsync();
 
            var finalResult =  _mapper.Map<IEnumerable<ProductFeatureViewModel>>(result);

            foreach(var fr in finalResult)
            {
                var product = products.FirstOrDefault(p => p.Id == fr.IdProduct);
                var category = categories.FirstOrDefault(c => c.Id == product.IdCategory);
                var feature = features.FirstOrDefault(f => f.Id == fr.IdFeature);

                fr.Feature = feature.Description;
                fr.Category = category.Name;
                fr.Feature = feature.Description;
                fr.Product = product.Brand + " - " + product.Model;
            }

            finalResult.OrderBy(r => r.Category)
                .ThenBy(r => r.Product)
                .ThenBy(r => r.Feature);


            return finalResult;
        
        }

        public async Task<ProductFeature> UpdateProductFeaturesAsync(ProductFeatureRequest model)
        {
            var ProductFeature = _mapper.Map<ProductFeature>(model);
            var result = await _productFeatureService.UpdateProductFeaturesAsync(ProductFeature);

            return result;
        }

    }
}
