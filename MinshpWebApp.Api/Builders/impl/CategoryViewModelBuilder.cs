using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using Category = MinshpWebApp.Domain.Models.Category;

namespace MinshpWebApp.Api.Builders.impl
{
    public class CategoryViewModelBuilder : ICategoryViewModelBuilder
    {
        private IMapper _mapper;
        private ICategoryService _categoryService;
        private ITaxeService _taxeService;
        private IPromotionCodeService _promotionCodeService;


        public CategoryViewModelBuilder(ICategoryService categoryService, ITaxeService taxeService, IPromotionCodeService promotionCodeService, IMapper mapper)
        {
            _mapper = mapper;
            _categoryService = categoryService;
            _taxeService = taxeService;
            _promotionCodeService = promotionCodeService;
        }

        public async Task<Category> AddCategorysAsync(CategoryRequest model)
        {
            var newCategory = _mapper.Map<Category>(model);
            return await _categoryService.AddCategorysAsync(newCategory);
        }

        public async Task<bool> DeleteCategorysAsync(int idCategory)
        {
            return await _categoryService.DeleteCategorysAsync(idCategory);
        }

        public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync()
        {
            var result = await _categoryService.GetCategoriesAsync();

            var list =  _mapper.Map<IEnumerable<CategoryViewModel>>(result);

            foreach (var item in list) {
                var promotionCodeLst = (await _promotionCodeService.GetPromotionCodesAsync()).Where(p => p.Id == item.IdPromotionCode).ToList();
                item.PromotionCodes = _mapper.Map<IEnumerable<PromotionCodeViewModel>>(promotionCodeLst) ;
                item.TaxeName = await GetTaxeName(item.TaxeId);
            
            }

            return list;
        }

        public async Task<Domain.Models.Category> UpdateCategorysAsync(CategoryRequest model)
        {
            return await _categoryService.UpdateCategorysAsync(_mapper.Map<Category>(model));
        }


        private async Task<List<string>> GetTaxeName(string idsTaxe)
        {
            string txs = idsTaxe;

            if (idsTaxe != null)
            {

                List<string> taxes = txs.Split(',')
                                          .Select(x => x.Trim()) // Supprime les espaces éventuels
                                          .ToList();

                List<string> finalTaxesNames = new List<string>();

                foreach (var t in taxes)
                {
                    var number = int.Parse(t);
                    var tax = (await _taxeService.GetTaxesAsync()).FirstOrDefault(t => t.Id == number);

                    //tax.Name = tax.Name.Split(':')[0].Trim();
                    finalTaxesNames.Add(tax.Name + ", ");
                }

                return finalTaxesNames;
            }
            return null;
        }
    }
}
