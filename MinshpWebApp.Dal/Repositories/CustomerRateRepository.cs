using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerRate = MinshpWebApp.Domain.Models.CustomerRate;

namespace MinshpWebApp.Dal.Repositories
{
    public class CustomerRateRepository : ICustomerRateRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public CustomerRateRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerRate>> GetCustomerRatesAsync()
        {
            var CustomerRateEntities = await _context.CustomerRates.Select(p => new CustomerRate
            {
                Id = p.Id,
                IdCustomer = p.IdCustomer,
                IdProduct = p.IdProduct,
                Rate = p.Rate,
                Title = p.Title,
                Message = p.Message,
                CreationDate = p.CreationDate,
                ModificationDate = p.ModificationDate
                
            }).ToListAsync();

            return CustomerRateEntities;
        }


        public async Task<CustomerRate> UpdateCustomerRateAsync(CustomerRate model)
        {
            var CustomerRateToUpdate = await _context.CustomerRates.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (CustomerRateToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.IdCustomer != null) CustomerRateToUpdate.IdCustomer = model.IdCustomer;
            if (model.IdProduct != null) CustomerRateToUpdate.IdProduct = model.IdProduct;
            if (model.Rate != null) CustomerRateToUpdate.Rate = model.Rate;
            if (model.Message != null) CustomerRateToUpdate.Message = model.Message;
            if (model.Title != null) CustomerRateToUpdate.Title = model.Title;
            CustomerRateToUpdate.ModificationDate = DateTime.Now;

            await _context.SaveChangesAsync();


            return new CustomerRate()
            {
                Id = model.Id,
                IdCustomer = model.IdCustomer,
                IdProduct = model.IdProduct,
                Rate = model.Rate,
                Message = model.Message,
                Title = model.Title
            };
        }


        public async Task<CustomerRate> AddCustomerRateAsync(Domain.Models.CustomerRate model)
        {
            var newCustomerRate = new Dal.Entities.CustomerRate
            {
                IdCustomer = model.IdCustomer,
                IdProduct = model.IdProduct,
                Rate = model.Rate,
                Message = model.Message,
                Title = model.Title,
                CreationDate = DateTime.Now
            };

            _context.CustomerRates.Add(newCustomerRate);
            _context.SaveChanges();

            return new CustomerRate()
            {
                IdCustomer = newCustomerRate.IdCustomer,
                IdProduct = newCustomerRate.IdProduct,
                Rate = newCustomerRate.Rate,
                Message = newCustomerRate.Message,
                Title = newCustomerRate.Title,
                CreationDate = newCustomerRate.CreationDate,
                ModificationDate = newCustomerRate.ModificationDate
            };
        }


        public async Task<bool> DeleteCustomerRateAsync(int idCustomerRate)
        {
            var CustomerRateToDelete = await _context.CustomerRates.FirstOrDefaultAsync(u => u.Id == idCustomerRate);

            if (CustomerRateToDelete == null)
                return false; // ou throw une exception;

            _context.CustomerRates.Remove(CustomerRateToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
