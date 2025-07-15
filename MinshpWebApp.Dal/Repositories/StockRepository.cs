using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stock = MinshpWebApp.Domain.Models.Stock;

namespace MinshpWebApp.Dal.Repositories
{
    public class StockRepository : IStockRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public StockRepository()
        {
            _context = new MinshpDatabaseContext();
        }

        public async Task<IEnumerable<Stock>> GetStocksAsync()
        {
            var StockEntities = await _context.Stocks.Select(p => new Stock
            {
                Id = p.Id,
                Quantity = p.Quantity,
                IdProduct = p.IdProduct
            }).ToListAsync();

            return StockEntities;
        }


        public async Task<Stock> UpdateStocksAsync(Stock model)
        {
            var StockToUpdate = await _context.Stocks.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (StockToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.Quantity != null) StockToUpdate.Quantity = model.Quantity;
            if (model.IdProduct != null) StockToUpdate.IdProduct = model.IdProduct;

            await _context.SaveChangesAsync();


            return new Stock()
            {
                Id = model.Id,
                Quantity = model.Quantity,
                IdProduct = model.IdProduct,
            };
        }


        public async Task<Stock> AddStocksAsync(Domain.Models.Stock model)
        {
            var newStock = new Dal.Entities.Stock
            {
                Id = model.Id,
                Quantity = model.Quantity,
                IdProduct = model.IdProduct
            };

            _context.Stocks.Add(newStock);
            _context.SaveChanges();

            return new Stock()
            {
                Id = model.Id,
                Quantity = model.Quantity,
                IdProduct = model.IdProduct
            };
        }


        public async Task<bool> DeleteStocksAsync(int idStock)
        {
            var StockToDelete = await _context.Stocks.FirstOrDefaultAsync(u => u.Id == idStock);

            if (StockToDelete == null)
                return false; // ou throw une exception;

            _context.Stocks.Remove(StockToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
