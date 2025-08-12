using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using MinshpWebApp.Domain.Services.impl;
using FeatureCategory = MinshpWebApp.Domain.Models.FeatureCategory;

namespace MinshpWebApp.Api.Builders.impl
{
    public class FeatureCategoryViewModelBuilder : IFeatureCategoryViewModelBuilder
    {


        private IMapper _mapper;
        private IFeatureCategoryService _featureCategoryService;
        private IFeatureService _featureService;
        private IProductFeatureService _productFeatureService;
        private IProductService _productService;

        public FeatureCategoryViewModelBuilder(IFeatureCategoryService featureCategoryService, IFeatureService featureService, IProductFeatureService productFeatureService, IProductService productService, IMapper mapper)
        {
            _mapper = mapper;
            _featureCategoryService = featureCategoryService;
            _featureService = featureService;
            _productFeatureService = productFeatureService;
            _productService = productService;
        }

        public async Task<Domain.Models.FeatureCategory> AddFeatureCategoryAsync(FeatureCategoryRequest model)
        {
            var newFeatureCategory = _mapper.Map<FeatureCategory>(model);
            return await _featureCategoryService.AddFeatureCategoryAsync(newFeatureCategory);
        }

        public async Task<bool> DeleteFeatureCategoryAsync(int idFeatureCategory)
        {
            return await _featureCategoryService.DeleteFeatureCategoryAsync(idFeatureCategory);
        }

        public async Task<IEnumerable<FeatureCategoryViewModel>> GetFeatureCategoriesAsync()
        {
            var featureCategories = await _featureCategoryService.GetFeatureCategoriesAsync();
            return _mapper.Map<IEnumerable<FeatureCategoryViewModel>>(featureCategories);
        }

        public async Task<FeatureCategory> UpdateFeatureCategoryAsync(FeatureCategoryRequest model)
        {
            var featureCategory = _mapper.Map<FeatureCategory>(model);

            return await _featureCategoryService.UpdateFeatureCategoryAsync(featureCategory);
        }


        public async Task<IEnumerable<FeaturesCategoryProductViewModel>> GetFeaturesCategoryProductAsync(int idProduct)
        {
            var product = (await _productService.GetProductsAsync()).FirstOrDefault(p => p.Id == idProduct);
            var featureCategories = await _featureCategoryService.GetFeatureCategoriesAsync();
            var featureProduct = (await _productFeatureService.GetProductFeaturesAsync()).Where(pf => pf.IdProduct == idProduct);
            var features = await _featureService.GetFeaturesAsync();
            var featuresCatProductList = new List<FeaturesCategoryProductViewModel>();

            var featureCategorieFilteredByProduct = (
                from fp in featureProduct
                join f in features on fp.IdFeature equals f.Id
                join fc in featureCategories on f.IdFeatureCategory equals fc.Id
                select fc).ToList();

            var featuresFilteredByProduct = (
                from pf in featureProduct        
                join f in features on pf.IdFeature equals f.Id 
                select f
                ).ToList();

       
            foreach (var fcn in featureCategorieFilteredByProduct)
            {
                var fcpwm = new FeaturesCategoryProductViewModel();

                fcpwm.IdFeatureCategory = fcn.Id;
                fcpwm.FeatureCategoryName = fcn.Name;

                var featuresDictionnary = new Dictionary<string, string>();
                foreach (var fc in featuresFilteredByProduct)
                {
                    if (fcn.Id == fc.IdFeatureCategory)
                    {
                        var valueToAddInDico = fc.Description.Split(':', 2);
                        
                        if(!featuresDictionnary.ContainsKey(valueToAddInDico[0]))
                            featuresDictionnary.Add(valueToAddInDico[0], valueToAddInDico[1]);
                    }
                }

                fcpwm.Specs = featuresDictionnary;

                bool b = featuresCatProductList?.Any(x => x.IdFeatureCategory == fcn.Id) == true;

                if (b == false)
                    featuresCatProductList.Add(fcpwm);
            }

            return featuresCatProductList;
        }
    }
}
