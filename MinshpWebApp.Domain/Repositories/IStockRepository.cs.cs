using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Repositories
{
    public interface IStockRepository
    {
        Task<IEnumerable<Stock>> GetStocksAsync();
        Task<Stock> UpdateStocksAsync(Stock model);
        Task<Stock> AddStocksAsync(Domain.Models.Stock model);
        Task<bool> DeleteStocksAsync(int idStock);
    }
}
