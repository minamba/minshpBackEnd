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
        private ISubCategoryService _subCategoryService;
        private IFeatureService _featureService;
        private IProductFeatureService _productFeature;
        private IImageService _imageService;
        private IPromotionService _promotionService;
        private IVideoService _videoService;
        private IStockService _stockService;
        private ITaxeService _taxeService;
        private IPromotionCodeService _promotionCodeService;
        private IPackageProfilService _packageProfilService;

        const string soonOutOfStock = "Bientôt en rupture";
        const string inStock = "En stock";
        const string outOfStock = "En rupture";

        public ProductViewModelBuilder(IProductService productService, ICategoryService categoryService, ISubCategoryService subCategoryService, IFeatureService featureService, IImageService imageService, IPromotionService promotionService, IVideoService videoService, IProductFeatureService productFeature, IStockService stockService, ITaxeService taxeService, IPromotionCodeService promotionCodeService, IPackageProfilService packageProfilService, IMapper mapper)
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
            _packageProfilService = packageProfilService;
            _subCategoryService = subCategoryService;
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
                    Price = Math.Round((decimal)p.Price, 2),
                    PriceTtc = await GetPriceTtc(p.IdCategory, p.Price),
                    PriceHtPromoted = await GetPriceHtPromoted(p, promotionList),
                    PriceHtCategoryCodePromoted = await GetPriceHtCategoryCodePromoted(p),
                    PriceHtSubCategoryCodePromoted = await GetPriceHtSubCategoryCodePromoted(p),
                    PurcentageCodePromoted = await GetPurcentageCodePromoted(p.Id, p.IdCategory, p.IdSubCategory, promotionList),
                    Tva = await GetTVA(p.IdCategory),
                    TaxWithoutTva = await GetTaxeWithoutTVA(p.IdCategory, p.IdSubCategory),
                    TaxWithoutTvaAmount = await GetTaxeAmountWithoutTVA(p.IdCategory, p.IdSubCategory),
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
                    Stocks = stockList,
                    IdPackageProfil = await GetIdPackageProfil(p),
                    PackageProfil = await GetPackageProfil(p.IdPackageProfil, p),
                    ContainedCode = await GetContainedCode(p),
                    IdSubCategory = p.IdSubCategory
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

            return Math.Round((decimal)priceTtc, 2);

        }

        private async Task<decimal?> GetPriceHtPromoted(ProductDto product, IEnumerable<PromotionViewModel> promotionList)
        {
            decimal? basePrice = product.Price;

            if (promotionList.ToList().Count>0)
            {
                var getPromotion = promotionList.ToList()[0];
                var purcentage = getPromotion.Purcentage;

                var promotedPrice = basePrice - (basePrice * (purcentage / 100m));

                return promotedPrice;
            }
            return null;
        }


        //CUMULABLE SI IL Y A DEJA UNE PROMO SUR LE PRODUIT
        private async Task<decimal?> GetPriceHtCategoryCodePromoted(ProductDto product)
        {
            var basePrice = product.Price;
            decimal? promotedCategoryCodePrice = 0;
            int? promotionPurcentage = 0;
            int? totalPurcentage = 0;

            var getPromotionCategoryCodeId = (await _categoryService.GetCategoriesAsync()).FirstOrDefault(c => c.Id == product.IdCategory && c.IdPromotionCode != 0).IdPromotionCode;
            var getPromotionPurcentage = (await _promotionService.GetPromotionsAsync()).FirstOrDefault(p => p.IdProduct == product.Id);

            if (getPromotionPurcentage != null && getPromotionPurcentage.EndDate >= DateTime.Now)
                promotionPurcentage = getPromotionPurcentage.Purcentage;
            

            //Dans le cas ou la promotion est faite la category
            if (getPromotionCategoryCodeId != null)
            {
                var getPormotionPurcentage = (await _promotionCodeService.GetPromotionCodesAsync()).FirstOrDefault(pr => pr.Id == getPromotionCategoryCodeId);

                if (getPormotionPurcentage != null)
                {
                    if (getPormotionPurcentage.Purcentage != null && getPormotionPurcentage.EndDate >= DateTime.Now)
                    {
                        totalPurcentage = promotionPurcentage + getPormotionPurcentage.Purcentage;
                        promotedCategoryCodePrice = Math.Round((decimal)(basePrice - (basePrice * (totalPurcentage / 100m))), 2);

                        return promotedCategoryCodePrice;
                    }
                }
            }
            return null;
        }

        //CUMULABLE SI IL Y A DEJA UNE PROMO SUR LE PRODUIT ET SUR LA CATEGORIE
        private async Task<decimal?> GetPriceHtSubCategoryCodePromoted(ProductDto product)
        {

            var basePrice = product.Price;
            int? promotionCategoryPurcentage = 0;
            int? promotionPurcentage = 0;
            int? totalPurcentage = 0;
            decimal? promotedSubCategoryCodePrice = 0;


            //je recupère le pourcentage de la promotion sur la catégory si il y en a un
            var getPromotionCategoryIdPromotionCode = (await _categoryService.GetCategoriesAsync()).FirstOrDefault(c => c.Id == product.IdCategory && c.IdPromotionCode != 0).IdPromotionCode;

            if (getPromotionCategoryIdPromotionCode != null)
            {
                var promotionCode = (await _promotionCodeService.GetPromotionCodesAsync()).FirstOrDefault(p => p.Id == getPromotionCategoryIdPromotionCode);

                if (promotionCode != null)
                {
                    if (promotionCode.EndDate >= DateTime.Now)
                        promotionCategoryPurcentage = promotionCode.Purcentage;
                }
            }

            //je recupère le pourcentage de la promotion sur le produit  si il y en a un
            var getPromotionPurcentage = (await _promotionService.GetPromotionsAsync()).FirstOrDefault(p => p.IdProduct == product.Id);
            if (getPromotionPurcentage != null && getPromotionPurcentage.EndDate >= DateTime.Now)
                promotionPurcentage = getPromotionPurcentage.Purcentage;


            if (product.IdSubCategory != null)
            {
                var getPromotionSubCategoryCodeId = (await _subCategoryService.GetSubCategoriesAsync()).FirstOrDefault(c => c.Id == product.IdSubCategory).IdPromotionCode;

                //Dans le cas ou la promotion est faite la sub category
                if (getPromotionSubCategoryCodeId != null)
                {
                    var getPormotionSubCategoryPurcentage = (await _promotionCodeService.GetPromotionCodesAsync()).FirstOrDefault(pr => pr.Id == getPromotionSubCategoryCodeId);

                    if (getPormotionSubCategoryPurcentage != null)
                    {
                        if (getPormotionSubCategoryPurcentage.Purcentage != null && getPormotionSubCategoryPurcentage.EndDate >= DateTime.Now)
                        {
                            if (promotionPurcentage != 0 && promotionCategoryPurcentage != 0)
                            {
                                totalPurcentage = promotionPurcentage + promotionCategoryPurcentage + getPormotionSubCategoryPurcentage.Purcentage;
                                promotedSubCategoryCodePrice = Math.Round((decimal)(basePrice - (basePrice * (totalPurcentage / 100m))), 2);
                            }
                            else if (promotionPurcentage == 0 && promotionCategoryPurcentage != 0)
                            {
                                totalPurcentage = promotionCategoryPurcentage + getPormotionSubCategoryPurcentage.Purcentage;
                                promotedSubCategoryCodePrice = Math.Round((decimal)(basePrice - (basePrice * (totalPurcentage / 100m))), 2);
                            }
                            else if (promotionPurcentage != 0 && promotionCategoryPurcentage == 0)
                            {
                                totalPurcentage = promotionPurcentage + getPormotionSubCategoryPurcentage.Purcentage;
                                promotedSubCategoryCodePrice = Math.Round((decimal)(basePrice - (basePrice * (totalPurcentage / 100m))), 2);
                            }
                            else
                                promotedSubCategoryCodePrice = Math.Round((decimal)(basePrice - (basePrice * (getPormotionSubCategoryPurcentage.Purcentage / 100m))), 2);

                            return promotedSubCategoryCodePrice;
                        }
                    }
                }
            }
            return null;
        }


        //POURCENTAGE CUMULABLE SI IL Y A DEJA UNE PROMO SUR LE PRODUIT
        private async Task<int?> GetPurcentageCodePromoted(int? productId, int? idCategory, int? idSubCatecory, IEnumerable<PromotionViewModel> promotionList)
        {
            int? totalPromotedCodePurcentage = null;
            var getPromotionCategoryCodeId = (await _categoryService.GetCategoriesAsync()).FirstOrDefault(c => c.Id == idCategory && c.IdPromotionCode != 0).IdPromotionCode;

            int? getPromotionSubCategoryCodeId = null;

            if(idSubCatecory != null)
                getPromotionSubCategoryCodeId = (await _subCategoryService.GetSubCategoriesAsync()).FirstOrDefault(c => c.Id == idSubCatecory && c?.IdPromotionCode != 0).IdPromotionCode;

            var getBasePurcentagePromotted = (await _promotionService.GetPromotionsAsync()).FirstOrDefault(p => p.IdProduct == productId);

            //Dans le cas ou la promotion est faite la category
            if (getPromotionCategoryCodeId != null && getPromotionSubCategoryCodeId == null)
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
            else if(getPromotionCategoryCodeId == null && getPromotionSubCategoryCodeId != null)
            {
                var getPormotionCodePurcentage = (await _promotionCodeService.GetPromotionCodesAsync()).FirstOrDefault(pr => pr.Id == getPromotionSubCategoryCodeId);


                if (getPormotionCodePurcentage != null && getPormotionCodePurcentage.EndDate >= DateTime.Now)
                {
                    if (getBasePurcentagePromotted != null && getBasePurcentagePromotted.EndDate >= DateTime.Now)
                        totalPromotedCodePurcentage = getPormotionCodePurcentage.Purcentage + getBasePurcentagePromotted.Purcentage;
                    else
                        totalPromotedCodePurcentage = getPormotionCodePurcentage.Purcentage;

                    return totalPromotedCodePurcentage;
                }
            }
            else if(getPromotionCategoryCodeId != null && getPromotionSubCategoryCodeId != null)
            {
                var getPormotionCodeCategoryPurcentage = (await _promotionCodeService.GetPromotionCodesAsync()).FirstOrDefault(pr => pr.Id == getPromotionCategoryCodeId);
                var getPormotionCodeSubCategoryPurcentage = (await _promotionCodeService.GetPromotionCodesAsync()).FirstOrDefault(pr => pr.Id == getPromotionSubCategoryCodeId);


                if ((getPormotionCodeCategoryPurcentage != null && getPormotionCodeCategoryPurcentage.EndDate >= DateTime.Now) && (getPormotionCodeSubCategoryPurcentage != null && getPormotionCodeSubCategoryPurcentage.EndDate >= DateTime.Now) )
                {
                    if (getBasePurcentagePromotted != null && getBasePurcentagePromotted.EndDate >= DateTime.Now)
                        totalPromotedCodePurcentage = getPormotionCodeCategoryPurcentage.Purcentage + getPormotionCodeSubCategoryPurcentage.Purcentage + getBasePurcentagePromotted.Purcentage;
                    else
                        totalPromotedCodePurcentage = getPormotionCodeCategoryPurcentage.Purcentage + getPormotionCodeSubCategoryPurcentage.Purcentage;

                    return totalPromotedCodePurcentage;
                }
            }

                return null;
        }

        private async Task<string?> GetTaxeWithoutTVA(int? idCategory, int? idSubCategory) 
        {
            string taxeEcoParticipation = null;
            var getFocusCategory = (await _categoryService.GetCategoriesAsync()).FirstOrDefault(c => c.Id == idCategory);
            var getFocusSubCategory = (await _subCategoryService.GetSubCategoriesAsync()).FirstOrDefault(c => c.Id == idSubCategory);

            List<string> taxes = getFocusCategory.IdTaxe.Split(',')
                                      .Select(x => x.Trim()) // Supprime les espaces éventuels
                                      .ToList();

 
            //TAXE DE BASE

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

            //TAXE COMPLEMENTAIRE (sous catégorie) : Cette va servir a recuperer les taxes (complémentaire) de la subCategory. On va faire une addition avec les taxes de la categorie. Les taxes de la catégorie parent est forcément appliquable sur l'enfant, mais l'inverse n'est pas vrai
            List<string> taxesSubCategory = new List<string>();

            if (getFocusSubCategory != null)
            {
                taxesSubCategory = getFocusSubCategory.IdTaxe.Split(',')
                                      .Select(x => x.Trim()) // Supprime les espaces éventuels
                                      .ToList();

                foreach (var t in taxesSubCategory)
                {
                    var idComplementaryTaxe = int.Parse(t);
                    var getComplementaryTaxe = (await _taxeService.GetTaxesAsync()).FirstOrDefault(t => t.Id == idComplementaryTaxe);

                    if (!getComplementaryTaxe.Name.ToLower().Contains("tva"))
                    {
                        if (getComplementaryTaxe.Name.ToLower().Contains("éco-participation"))
                        {
                            taxeEcoParticipation = taxeEcoParticipation + " et " + getComplementaryTaxe.Name.Split(":")[0] + "sur" + getComplementaryTaxe.Name.Split(":")[1] + " " + getComplementaryTaxe.Amount.ToString() + "€";
                        }
                    }
                }

            }

            return taxeEcoParticipation;
        }


        private async Task<decimal?> GetTaxeAmountWithoutTVA(int? idCategory, int? idSubCategory)
        {
            decimal? taxeEcoParticipation = null;
            var getFocusCategory = (await _categoryService.GetCategoriesAsync()).FirstOrDefault(c => c.Id == idCategory);
            var getFocusSubCategory = (await _subCategoryService.GetSubCategoriesAsync()).FirstOrDefault(c => c.Id == idSubCategory);

            List<string> taxes = getFocusCategory.IdTaxe.Split(',')
                                      .Select(x => x.Trim()) // Supprime les espaces éventuels
                                      .ToList();


            //TAXE DE BASE

            foreach (var t in taxes)
            {
                var idTaxe = int.Parse(t);
                var getTaxe = (await _taxeService.GetTaxesAsync()).FirstOrDefault(t => t.Id == idTaxe);

                if (!getTaxe.Name.ToLower().Contains("tva"))
                {
                    if (getTaxe.Name.ToLower().Contains("éco-participation"))
                    {
                        taxeEcoParticipation =  getTaxe.Amount;
                    }
                }
            }

            //TAXE COMPLEMENTAIRE (sous catégorie) : Cette va servir a recuperer les taxes (complémentaire) de la subCategory. On va faire une addition avec les taxes de la categorie. Les taxes de la catégorie parent est forcément appliquable sur l'enfant, mais l'inverse n'est pas vrai
            List<string> taxesSubCategory = new List<string>();

            if (getFocusSubCategory != null)
            {
                taxesSubCategory = getFocusSubCategory.IdTaxe.Split(',')
                                      .Select(x => x.Trim()) // Supprime les espaces éventuels
                                      .ToList();

                foreach (var t in taxesSubCategory)
                {
                    var idComplementaryTaxe = int.Parse(t);
                    var getComplementaryTaxe = (await _taxeService.GetTaxesAsync()).FirstOrDefault(t => t.Id == idComplementaryTaxe);

                    if (!getComplementaryTaxe.Name.ToLower().Contains("tva"))
                    {
                        if (getComplementaryTaxe.Name.ToLower().Contains("éco-participation"))
                        {
                            taxeEcoParticipation += getComplementaryTaxe.Amount;
                        }
                    }
                }

            }

            return taxeEcoParticipation;
        }


        private async Task<int?> GetTVA(int? idCategory)
        {
            string tva = null;
            var getFocusCategory = (await _categoryService.GetCategoriesAsync()).FirstOrDefault(c => c.Id == idCategory);
            var getFocusSubCategory = (await _subCategoryService.GetSubCategoriesAsync()).FirstOrDefault(c => c.Id == idCategory);

            List<string> taxes = getFocusCategory.IdTaxe.Split(',')
                                      .Select(x => x.Trim()) // Supprime les espaces éventuels
                                      .ToList();


            //TAXE DE BASE

            foreach (var t in taxes)
            {
                var idTaxe = int.Parse(t);
                var getTaxe = (await _taxeService.GetTaxesAsync()).FirstOrDefault(t => t.Id == idTaxe);

                if (getTaxe.Name.ToLower().Contains("tva"))
                {
                    return getTaxe.Purcentage;
                }
            }

            return null;
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


        private async Task<PackageProfilViewModel> GetPackageProfil(int? id, ProductDto product)
        {
            Domain.Models.PackageProfil packgeProfil = null;


            if(id != null)
             packgeProfil = (await _packageProfilService.GetPackageProfilsAsync()).FirstOrDefault(pa => pa.Id == id);
            else
            {
                var getCategory = (await _categoryService.GetCategoriesAsync()).FirstOrDefault(c => c.Id == product.IdCategory);
                packgeProfil = (await _packageProfilService.GetPackageProfilsAsync()).FirstOrDefault(pa => pa.Id == getCategory.IdPackageProfil);
            }

                return _mapper.Map<PackageProfilViewModel>(packgeProfil);
        }



        private async Task<int?> GetIdPackageProfil(ProductDto product)
        {
            int? packgeProfil = null;


            if(product != null)
            {
                var getCategory = (await _categoryService.GetCategoriesAsync()).FirstOrDefault(c => c.Id == product.IdCategory);
                packgeProfil = (await _packageProfilService.GetPackageProfilsAsync()).FirstOrDefault(pa => pa.Id == getCategory.IdPackageProfil).Id;
            }

            return packgeProfil;
        }


        private async Task<string?> GetContainedCode(ProductDto product)
        {
            string? containedCode = null;

            if (product != null)
                containedCode = (await _categoryService.GetCategoriesAsync()).FirstOrDefault(c => c.Id == product.IdCategory).ContentCode.ToString();
            
            return containedCode;
        }
    }


}
