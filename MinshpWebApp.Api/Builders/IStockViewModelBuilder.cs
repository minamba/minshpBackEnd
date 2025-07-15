using MinshpWebApp.Api.Builders.impl;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IStockViewModelBuilder
    {
        Task<IEnumerable<StockViewModel>> GetStocksAsync();
        Task<Stock> UpdateStocksAsync(StockRequest model);
        Task<Stock> AddStocksAsync(StockRequest model);
        Task<bool> DeleteStocksAsync(int idStock);
    }
}
