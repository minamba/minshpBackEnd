using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using MinshpWebApp.Domain.Services.impl;
using Stock = MinshpWebApp.Domain.Models.Stock;


namespace MinshpWebApp.Api.Builders.impl
{
    public class StockViewModelBuilder : IStockViewModelBuilder
    {
        private IMapper _mapper;
        private IStockService _stockService;


        public StockViewModelBuilder(IStockService stockService, IMapper mapper)
        {
            _mapper = mapper;
            _stockService = stockService;
        }


        public async Task<Stock> AddStocksAsync(StockRequest model)
        {
            return await _stockService.AddStocksAsync(_mapper.Map<Stock>(model));
        }

        public async Task<bool> DeleteStocksAsync(int idStock)
        {
            return await _stockService.DeleteStocksAsync(idStock);
        }

        public async Task<IEnumerable<StockViewModel>> GetStocksAsync()
        {
            var result = await _stockService.GetStocksAsync();

            return _mapper.Map<IEnumerable<StockViewModel>>(result);
        }

        public async Task<Stock> UpdateStocksAsync(StockRequest model)
        {
            var stock = _mapper.Map<Stock>(model);
            var result = await _stockService.UpdateStocksAsync(stock);

            return result;
        }
    }
}
