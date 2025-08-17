using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using MinshpWebApp.Domain.Services.impl;

namespace MinshpWebApp.Api.Builders.impl
{
    public class TaxeViewModelBuilder : ITaxeViewModelBuilder
    {

        private IMapper _mapper;
        private ITaxeService _taxeService;


        public TaxeViewModelBuilder(ITaxeService taxeService, IMapper mapper)
        {
            _mapper = mapper;
            _taxeService = taxeService;
        }

        public async Task<Taxe> AddTaxeAsync(TaxeRequest model)
        {
            var newTaxe = _mapper.Map<Taxe>(model);
            return await _taxeService.AddTaxeAsync(newTaxe);
        }


        public async Task<bool> DeleteTaxeAsync(int idTaxe)
        {
            return await _taxeService.DeleteTaxeAsync(idTaxe);
        }


        public async Task<IEnumerable<TaxeViewModel>> GetTaxesAsync()
        {
            var Taxes = await _taxeService.GetTaxesAsync();

            return _mapper.Map<IEnumerable<TaxeViewModel>>(Taxes);
        }

        public async Task<Taxe> UpdateTaxeAsync(TaxeRequest model)
        {
            var Taxe = _mapper.Map<Taxe>(model);

            return await _taxeService.UpdateTaxeAsync(Taxe);
        }
    }
}
