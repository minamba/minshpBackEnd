using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using MinshpWebApp.Domain.Services.impl;

namespace MinshpWebApp.Api.Builders.impl
{
    public class ProductViewModelBuilder : IProductViewModelBuilder
    {
        private IMapper _mapper;
        private IProductService _productService;
        private ICategoryService _categoryService;
        private IFeatureService _featureService;
        private IProductFeatureService _productFeature;
        private IImageService _imageService;
        private IPromotionService _promotionService;
        private IVideoService _videoService;


        public ProductViewModelBuilder(IProductService productService, ICategoryService categoryService, IFeatureService featureService, IImageService imageService, IPromotionService promotionService, IVideoService videoService, IProductFeatureService productFeature, IMapper mapper)
        {
            _productService = productService;
            _categoryService = categoryService;
            _featureService = featureService;
            _imageService = imageService;
            _promotionService = promotionService;
            _videoService = videoService;
            _productFeature = productFeature;
            _mapper = mapper;
               
        }

        public async Task<IEnumerable<ProductVIewModel>> GetProductsAsync()
        {

            var products = await _productService.GetProductsAsync();
            var images = await _imageService.GetImagesAsync();
            var promotions = await _promotionService.GetPromotionsAsync();
            var videos = await _videoService.GetVideosAsync();
            var features = await _featureService.GetFeaturesAsync();
            var categories = await _categoryService.GetCategoriesAsync();
            var featuresProduct = await _productFeature.GetProductFeaturesAsync();

            var productVmList = new List<ProductVIewModel>();

            foreach (var p in products)
            {

                var categoryName = categories.Where(c => c.Id == p.IdCategory).Select(c => c.Name).FirstOrDefault();

                //get features
                var featuresForProduct = (from fp in featuresProduct
                                         join f in features on fp.IdFeature equals f.Id
                                         where fp.IdProduct == p.Id
                                         select f).ToList();

                var featuresList = _mapper.Map<IEnumerable<FeatureViewModel>>(featuresForProduct);

                
                //get images
                var imagesForProduct = images.Where(i => i.IdProduct == p.Id).ToList();
                var imageList = _mapper.Map<IEnumerable<ImageViewModel>>(imagesForProduct);

                //get promotions
                var promotionsForProduct = promotions.Where(i => i.IdProduct == p.Id).ToList();
                var promotionList = _mapper.Map<IEnumerable<PromotionViewModel>>(promotionsForProduct);

                //GetHashCode videos
                var videosForProduct = videos.Where(i => i.IdProduct == p.Id).ToList();
                var videosList = _mapper.Map<IEnumerable<VideoViewModel>>(videosForProduct);

                var productVm = new ProductVIewModel()
                {
                     Id = p.Id,
                     Name = p.Name,
                     Description = p.Description,
                     Price = p.Price,
                     Category = categoryName,
                     Features = featuresList,
                     Images = imageList,
                     Promotions = promotionList,
                     Videos = videosList
                };

                productVmList.Add(productVm);
            }

            return productVmList;
        }

        public async Task<Product> UpdateProductsAsync(ProductRequest model)
        {
            return await _productService.UpdateProductsAsync(_mapper.Map<Product>(model));
        }

        public async Task<Product> AddProductsAsync(ProductRequest model)
        {
            return await _productService.AddProductsAsync(_mapper.Map<Product>(model));
        }

        public async Task<bool> DeleteProductsAsync(int idProduct)
        {
            return await _productService.DeleteProductsAsync(idProduct);
        }
    }
}
