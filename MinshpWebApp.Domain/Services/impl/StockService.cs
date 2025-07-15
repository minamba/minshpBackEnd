using AutoMapper;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class StockService : IStockService
    {
        private IMapper _mapper;
        private IStockRepository _repository;


        public StockService(IStockRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }


        public async Task<Stock> AddStocksAsync(Stock model)
        {
            return await _repository.AddStocksAsync(model);
        }

        public async Task<bool> DeleteStocksAsync(int idStock)
        {
           return await _repository.DeleteStocksAsync(idStock);
        }

        public async Task<IEnumerable<Stock>> GetStocksAsync()
        {
           return await _repository.GetStocksAsync();
        }

        public async Task<Stock> UpdateStocksAsync(Stock model)
        {
           return await _repository.UpdateStocksAsync(model);
        }
    }
}
