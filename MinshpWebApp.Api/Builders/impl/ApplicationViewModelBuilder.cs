using AutoMapper;
using Microsoft.IdentityModel.Tokens.Experimental;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Dtos;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using System.Globalization;

namespace MinshpWebApp.Api.Builders.impl
{
    public class ApplicationViewModelBuilder : IApplicationViewModelBuilder
    {
        private IMapper _mapper;
        private IApplicationService _applicationService;
        private IPromotionCodeService _promotionCodeService;
        private IPromotionService _promotionService;
        private IProductService _productService;
        private ICategoryService _categoryService;
        private ISubCategoryService _subCategoryService;


        public ApplicationViewModelBuilder(IApplicationService applicationService, IPromotionCodeService promotionCodeService, IPromotionService promotionService, IProductService productService, ICategoryService categoryService, ISubCategoryService subCategoryService, IMapper mapper)
        {
            _mapper = mapper;
            _applicationService = applicationService;
            _promotionCodeService = promotionCodeService;
            _promotionService = promotionService;
            _productService = productService;
            _categoryService = categoryService;
            _subCategoryService = subCategoryService;
        }

        public async Task<Application> AddApplicationsAsync(ApplicationRequest model)
        {
            return await _applicationService.AddApplicationsAsync(_mapper.Map<Application>(model));
        }

        public async Task<bool> DeleteApplicationsAsync(int idApplication)
        {
            return await _applicationService.DeleteApplicationsAsync(idApplication);
        }

        public async Task<IEnumerable<ApplicationViewModel>> GetApplicationAsync()
        {
            var products = (await _productService.GetProductsAsync());
            var categories = (await _categoryService.GetCategoriesAsync()).Where(c => c.IdPromotionCode != null);
            var subCategories = (await _subCategoryService.GetSubCategoriesAsync()).Where(c => c.IdPromotionCode != null);
            var promotions = await _promotionService.GetPromotionsAsync();
            var promotionCodes = await _promotionCodeService.GetPromotionCodesAsync();

            var result = await _applicationService.GetApplicationAsync();

            var applicetions = _mapper.Map<IEnumerable<ApplicationViewModel>>(result);

            foreach (var a in applicetions)
            {
                a.PromoMessages = await GetPromoMessages(products, categories, subCategories, promotions, promotionCodes);
            }

            return applicetions;
        }

        public async Task<Application> UpdateApplicationsAsync(ApplicationRequest model)
        {
            var application = _mapper.Map<Application>(model);
            var result = await _applicationService.UpdateApplicationsAsync(application);

            return result;
        }


        private async Task<List<string>> GetPromoMessages(IEnumerable<ProductDto> products, IEnumerable<Category> categories, IEnumerable<SubCategory> subCategories, IEnumerable<Promotion> promotions, IEnumerable<PromotionCode> promotionCodes)
        {

            List<string> promoMessages = new List<string>();

            if(products != null)
            {
                foreach(var p in products)
                {
                    if (p.IdPromotionCode != null)
                    {
                        var getPromotionCode = promotionCodes.FirstOrDefault(pc => pc.Id == p.IdPromotionCode);
                        var message = getPromotionCode.Purcentage.ToString() + "% sur le produit '" + p.Brand + " " + p.Model + "' avec le code '" + getPromotionCode.Name.ToUpper() + "' jusqu'au " + getPromotionCode.EndDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                        if(getPromotionCode.EndDate >= DateTime.Now)
                            promoMessages.Add(message);
                    }
                }
            }


            if (categories != null)
            {
                foreach (var c in categories)
                {
                    if (c.IdPromotionCode != null)
                    {
                        var getPromotionCode = promotionCodes.FirstOrDefault(pc => pc.Id == c.IdPromotionCode);
                        var message = getPromotionCode.Purcentage.ToString() + "% sur les '" + c.Name + "' jusqu'au " + getPromotionCode.EndDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                        if (getPromotionCode.EndDate >= DateTime.Now)
                            promoMessages.Add(message);
                    }
                }
            }


            if (subCategories != null)
            {
                foreach (var sc in subCategories)
                {
                    if (sc.IdPromotionCode != null)
                    {
                        var getPromotionCode = promotionCodes.FirstOrDefault(pc => pc.Id == sc.IdPromotionCode);
                        var message = getPromotionCode.Purcentage.ToString() + "% sur les '" + sc.Name + "' jusqu'au " + getPromotionCode.EndDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        
                        if (getPromotionCode.EndDate >= DateTime.Now)
                            promoMessages.Add(message);
                    }
                }
            }


            if (promotions != null)
            {
                foreach (var pr in promotions)
                {
                    if (pr.IdProduct != null)
                    {
                        var getProduct = products.FirstOrDefault(p => p.Id == pr.IdProduct);
                        var message = pr.Purcentage.ToString() + "% sur '" + getProduct.Brand + " " + getProduct.Model + "' jusqu'au " + pr.EndDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                        if (pr.EndDate >= DateTime.Now)
                            promoMessages.Add(message);
                    }
                }
            }

            promoMessages.Add("Envoie rapide sous 24h");

            return promoMessages;
        }
    }
}
