using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using MinshpWebApp.Domain.Services.impl;

namespace MinshpWebApp.Api.Builders.impl
{
    public class SubCategoryViewModelBuilder : ISubCategoryViewModelBuilder
    {
        private IMapper _mapper;
        private ISubCategoryService _subCategoryService;
        private ITaxeService _taxeService;
        private IPromotionCodeService _promotionCodeService;


        public SubCategoryViewModelBuilder(ISubCategoryService subCategoryService, IMapper mapper, IPromotionCodeService promotionCodeService, ITaxeService taxeService)
        {
            _mapper = mapper;
            _subCategoryService = subCategoryService;
            _promotionCodeService = promotionCodeService;
            _taxeService = taxeService;
        }

        public async Task<SubCategory> AddSubCategorysAsync(SubCategoryRequest model)
        {
            var newSubCategory = _mapper.Map<SubCategory>(model);
            return await _subCategoryService.AddSubCategorysAsync(newSubCategory);
        }

        public async Task<bool> DeleteSubCategorysAsync(int idSubCategory)
        {
            return await _subCategoryService.DeleteSubCategorysAsync(idSubCategory);
        }

        public async Task<IEnumerable<SubCategoryViewModel>> GetSubCategoriesAsync()
        {
            var result = await _subCategoryService.GetSubCategoriesAsync();

            var list = _mapper.Map<IEnumerable<SubCategoryViewModel>>(result);

            foreach (var item in list)
            {
                var promotionCodeLst = (await _promotionCodeService.GetPromotionCodesAsync()).Where(p => p.Id == item.IdPromotionCode).ToList();
                item.PromotionCodes = _mapper.Map<IEnumerable<PromotionCodeViewModel>>(promotionCodeLst);
                item.TaxeName = await GetTaxeName(item.IdTaxe);

            }

            return list;
        }

        public async Task<Domain.Models.SubCategory> UpdateSubCategorysAsync(SubCategoryRequest model)
        {
            return await _subCategoryService.UpdateSubCategorysAsync(_mapper.Map<SubCategory>(model));
        }


        private async Task<List<string>> GetTaxeName(string idsTaxe)
        {
            string txs = idsTaxe;
            List<string> taxes = new List<string>();

            if (txs != null)
            {
                taxes = txs.Split(',')
                                           .Select(x => x.Trim()) // Supprime les espaces éventuels
                                           .ToList();

                List<string> finalTaxesNames = new List<string>();
                Taxe tax = null;
                var taxesLst = await _taxeService.GetTaxesAsync();

                foreach (var t in taxes)
                {
                    var number = int.Parse(t);

                    tax = taxesLst.FirstOrDefault(t => t.Id == number);
                    //tax.Name = tax.Name.Split(':')[0].Trim();
                    finalTaxesNames.Add(tax.Name + ", ");

                }

                return finalTaxesNames;
            }
            return null;
        }
    }
}
