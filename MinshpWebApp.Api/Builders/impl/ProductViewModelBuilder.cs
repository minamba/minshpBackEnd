using AutoMapper;
using Azure;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Dtos;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using MinshpWebApp.Domain.Services.impl;
using System.Collections.Generic;
using System.Linq;
using Product = MinshpWebApp.Domain.Models.Product;
using Taxe = MinshpWebApp.Domain.Models.Taxe;

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
        private IStockService _stockService;
        private ITaxeService _taxeService;
        private IPromotionCodeService _promotionCodeService;

        const string soonOutOfStock = "Bientôt en rupture";
        const string inStock = "En stock";
        const string outOfStock = "En rupture";

        public ProductViewModelBuilder(IProductService productService, ICategoryService categoryService, IFeatureService featureService, IImageService imageService, IPromotionService promotionService, IVideoService videoService, IProductFeatureService productFeature, IStockService stockService, ITaxeService taxeService, IPromotionCodeService promotionCodeService, IMapper mapper)
        {
            _productService = productService;
            _categoryService = categoryService;
            _featureService = featureService;
            _imageService = imageService;
            _promotionService = promotionService;
            _videoService = videoService;
            _productFeature = productFeature;
            _stockService = stockService;
            _taxeService = taxeService;
            _promotionCodeService = promotionCodeService;
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
            var stocks = await _stockService.GetStocksAsync();

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

                //get stocks
                var stocksForProduct = stocks.Where(i => i.IdProduct == p.Id).FirstOrDefault();
                var stockList = _mapper.Map<StockViewModel>(stocksForProduct);

                //GetHashCode videos
                var videosForProduct = videos.Where(i => i.IdProduct == p.Id).ToList();
                var videosList = _mapper.Map<IEnumerable<VideoViewModel>>(videosForProduct);

                var productVm = new ProductVIewModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    PriceTtc = await GetPriceTtc(p.IdCategory,p.Price),
                    PriceTtcPromoted = await GetPriceTtcPromoted(await GetPriceTtc(p.IdCategory, p.Price), promotionList),
                    PriceTtcCategoryCodePromoted = await GetPriceTtcCategoryCodePromoted(await GetPriceTtc(p.IdCategory, p.Price),p.IdCategory, promotionList),
                    PurcentageCodePromoted = await GetPurcentageCodePromoted(p.Id, p.IdCategory,promotionList),
                    TaxWithoutTvaAmount = await GetTaxeAmountWithoutTVA(p.IdCategory),
                    Category = categoryName,
                    Main = p.Main,
                    Brand = p.Brand,
                    Model = p.Model,
                    StockStatus = await GetStockStatus(stockList),
                    CreationDate = p.CreationDate,
                    ModificationDate = p.ModificationDate,
                    IdPromotionCode = p.IdPromotionCode,
                    Features = featuresList,
                    Images = imageList,
                    Promotions = promotionList,
                    Videos = videosList,
                    Stocks = stockList
                };

                productVmList.Add(productVm);
            }

            return productVmList;
        }

        public async Task<Product> UpdateProductsAsync(ProductRequest model)
        {
            model.ModificationDate = DateTime.Now;
            var products = (await _productService.GetProductsAsync()).ToList();

            if (model.Main == true)
            {
                foreach (var item in products)
                {
                    if (item.Id != model.Id)
                    {
                        item.Main = false;
                        await _productService.UpdateProductsAsync(_mapper.Map<Product>(item));
                    }
                }
            }

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


        private async Task<decimal?> GetPriceTtc(int? idCategory,decimal? price)
        {
            decimal? priceTtc = 0;
            var getFocusCategory = (await _categoryService.GetCategoriesAsync()).FirstOrDefault(c => c.Id == idCategory);

            List<string> taxes = getFocusCategory.IdTaxe.Split(',')
                                      .Select(x => x.Trim()) // Supprime les espaces éventuels
                                      .ToList();

            List<Taxe> taxesList = new List<Taxe>();

            foreach (var t in taxes) 
            {
                var idTaxe = int.Parse(t);
                var getTaxe = (await _taxeService.GetTaxesAsync()).FirstOrDefault(t => t.Id == idTaxe);

                taxesList.Add(getTaxe);
            }

            foreach (var t in taxesList) 
            {
                if (t.Purcentage != null)
                    priceTtc = (price * (t.Purcentage / 100m)) + price;

                if (t.Amount != null)
                    priceTtc += t.Amount;

            }

            return priceTtc;

        }

        private async Task<decimal?> GetPriceTtcPromoted(decimal? priceTtc, IEnumerable<PromotionViewModel> promotionList)
        {
            if (promotionList.ToList().Count>0)
            {
                var getPromotion = promotionList.ToList()[0];
                var purcentage = getPromotion.Purcentage;

                var promotedPrice = priceTtc - (priceTtc * (purcentage / 100m));

                return promotedPrice;
            }

            return null;
        }


        //CUMULABLE SI IL Y A DEJA UNE PROMO SUR LE PRODUIT
        private async Task<decimal?> GetPriceTtcCategoryCodePromoted(decimal? priceTtc, int? idCategory, IEnumerable<PromotionViewModel> promotionList)
        {
            decimal? promotedCategoryCodePrice = 0;
            var getPromotionCategoryCodeId = (await _categoryService.GetCategoriesAsync()).FirstOrDefault(c => c.Id == idCategory && c.IdPromotionCode != 0).IdPromotionCode;

            var getPricePromotted = await GetPriceTtcPromoted(priceTtc, promotionList);

            //Dans le cas ou la promotion est faite la category
            if (getPromotionCategoryCodeId != null)
            {
                var getPormotionPurcentage = (await _promotionCodeService.GetPromotionCodesAsync()).FirstOrDefault(pr => pr.Id == getPromotionCategoryCodeId);

                if (getPormotionPurcentage != null)
                {
                    if (getPormotionPurcentage.Purcentage != null && getPormotionPurcentage.EndDate >= DateTime.Now)
                    {
                        if (getPricePromotted != null)
                            promotedCategoryCodePrice = getPricePromotted - (getPricePromotted * (getPormotionPurcentage.Purcentage / 100m));
                        else
                            promotedCategoryCodePrice = priceTtc - (priceTtc * (getPormotionPurcentage.Purcentage / 100m));

                        return promotedCategoryCodePrice;
                    }
                }
            }

            return null;
        }


        //POURCENTAGE CUMULABLE SI IL Y A DEJA UNE PROMO SUR LE PRODUIT
        private async Task<int?> GetPurcentageCodePromoted(int? productId, int? idCategory, IEnumerable<PromotionViewModel> promotionList)
        {
            int? totalPromotedCodePurcentage = null;
            var getPromotionCategoryCodeId = (await _categoryService.GetCategoriesAsync()).FirstOrDefault(c => c.Id == idCategory && c.IdPromotionCode != 0).IdPromotionCode;

            var getBasePurcentagePromotted = (await _promotionService.GetPromotionsAsync()).FirstOrDefault(p => p.IdProduct == productId);

            //Dans le cas ou la promotion est faite la category
            if (getPromotionCategoryCodeId != null)
            {
                var getPormotionCodePurcentage = (await _promotionCodeService.GetPromotionCodesAsync()).FirstOrDefault(pr => pr.Id == getPromotionCategoryCodeId);


                if (getPormotionCodePurcentage != null && getPormotionCodePurcentage.EndDate >= DateTime.Now)
                {
                    if(getBasePurcentagePromotted != null && getBasePurcentagePromotted.EndDate >= DateTime.Now)
                        totalPromotedCodePurcentage = getPormotionCodePurcentage.Purcentage + getBasePurcentagePromotted.Purcentage;
                    else
                        totalPromotedCodePurcentage = getPormotionCodePurcentage.Purcentage;
             
                        return totalPromotedCodePurcentage;
                }
            }

            return null;
        }

        private async Task<string?> GetTaxeAmountWithoutTVA(int? idCategory) 
        {
            string taxeEcoParticipation = null;
            var getFocusCategory = (await _categoryService.GetCategoriesAsync()).FirstOrDefault(c => c.Id == idCategory);

            List<string> taxes = getFocusCategory.IdTaxe.Split(',')
                                      .Select(x => x.Trim()) // Supprime les espaces éventuels
                                      .ToList();

            foreach (var t in taxes)
            {
                var idTaxe = int.Parse(t);
                var getTaxe = (await _taxeService.GetTaxesAsync()).FirstOrDefault(t => t.Id == idTaxe);

                if(!getTaxe.Name.ToLower().Contains("tva"))
                {
                    if(getTaxe.Name.ToLower().Contains("éco-participation"))
                    {
                        taxeEcoParticipation = "dont " + getTaxe.Name.Split(":")[0] + getTaxe.Amount.ToString() + "€";
                    }
                }
            }

            return taxeEcoParticipation;
        }

        private async Task<string?> GetStockStatus(StockViewModel stockList)
        {
            var result = stockList.Quantity switch
            {
                0 => outOfStock,
                <= 5 => "Plus que " + stockList.Quantity + " produits en stock",
                <= 10 => soonOutOfStock,
                > 10 => inStock,
                _ => "En rupture"
            };

            return result;
        }
    }


}
