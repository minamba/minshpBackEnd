using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using PromotionCode = MinshpWebApp.Domain.Models.PromotionCode;

namespace MinshpWebApp.Api.Builders.impl
{
    public class PromotionCodeViewModelBuilder : IPromotionCodeViewModelBuilder
    {
        private IMapper _mapper;
        private IPromotionCodeService _promotionCodeService;
        private IProductViewModelBuilder _productVm;
        private ICategoryViewModelBuilder _categoryVm;


        public PromotionCodeViewModelBuilder(IPromotionCodeService promotionCodeService, IProductViewModelBuilder productVm, ICategoryViewModelBuilder categoryVm, IMapper mapper)
        {
            _mapper = mapper;
            _promotionCodeService = promotionCodeService;
            _productVm = productVm;
            _categoryVm = categoryVm;
        }


        public async Task<PromotionCode> AddPromotionCodesAsync(PromotionCodeRequest model)
        {
           var result = await _promotionCodeService.AddPromotionCodesAsync(_mapper.Map<PromotionCode>(model));

            var getProduct = (await _productVm.GetProductsAsync()).FirstOrDefault(p => p.Id == model.IdProduct);
            var getCategory = (await _categoryVm.GetCategoriesAsync()).FirstOrDefault(p => p.Id == model.IdCategory);


            if (model.IdProduct != null && model.IdCategory != null)
            {
                var productRequest = new ProductRequest();
                productRequest.Id = getProduct.Id;
                productRequest.IdPromotionCode = result.Id;
                await _productVm.UpdateProductsAsync(productRequest);
            }
            else if (model.IdCategory != null && model.IdProduct == null)
            {
                var categoryRequest = new CategoryRequest();
                categoryRequest.Id = getCategory.Id;
                categoryRequest.IdPromotionCode = result.Id;
                await _categoryVm.UpdateCategorysAsync(categoryRequest);
            }

            return result;
        }

        public async Task<bool> DeletePromotionCodesAsync(int idPromotionCode)
        {
            var result = await _promotionCodeService.DeletePromotionCodesAsync(idPromotionCode);

            var getProduct = (await _productVm.GetProductsAsync()).FirstOrDefault(p => p.IdPromotionCode == idPromotionCode);
            var getCategory = (await _categoryVm.GetCategoriesAsync()).FirstOrDefault(p => p.IdPromotionCode == idPromotionCode);

            if (getProduct != null)
            {
                var productRequest = new ProductRequest();
                productRequest.Id = getProduct.Id;
                productRequest.IdPromotionCode = idPromotionCode;
                await _productVm.UpdateProductsAsync(productRequest);
            }


            if (getCategory != null)
            {
                var categoryRequest = new CategoryRequest();
                categoryRequest.Id = getCategory.Id;
                categoryRequest.IdPromotionCode = idPromotionCode;
                await _categoryVm.UpdateCategorysAsync(categoryRequest);
            }


            return result;
        }

        public async Task<IEnumerable<PromotionCodeViewModel>> GetPromotionCodesAsync()
        {
            var result = await _promotionCodeService.GetPromotionCodesAsync();

            var list = _mapper.Map<IEnumerable<PromotionCodeViewModel>>(result);

            foreach (var item in list) 
            {
                var getProudct = (await _productVm.GetProductsAsync()).FirstOrDefault(p => p.IdPromotionCode == item.Id);
                var getCategory = (await _categoryVm.GetCategoriesAsync()).FirstOrDefault(p => p.IdPromotionCode == item.Id);
                item.Product = getProudct;
                item.Category = getCategory;
            }

            return list;
        }

        public async Task<PromotionCode> UpdatePromotionCodesAsync(PromotionCodeRequest model)
        {
            var promotionCode = _mapper.Map<PromotionCode>(model);
            var result = await _promotionCodeService.UpdatePromotionCodesAsync(promotionCode);

            return result;
        }
    }
}
