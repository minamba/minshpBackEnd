using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerPromotionCode = MinshpWebApp.Domain.Models.CustomerPromotionCode;

namespace MinshpWebApp.Dal.Repositories
{
    public class CustomerPromotionCodeRepository : ICustomerPromotionCodeRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public CustomerPromotionCodeRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerPromotionCode>> GetCustomerPromotionCodesAsync()
        {
            var CustomerPromotionCodeEntities = await _context.CustomerPromotionCodes.Select(p => new CustomerPromotionCode
            {
                Id = p.Id,
                IdCutomer = p.IdCutomer,
                IdPromotion = p.IdPromotion,
                IsUsed = p.IsUsed
            }).ToListAsync();

            return CustomerPromotionCodeEntities;
        }


        public async Task<CustomerPromotionCode> UpdateCustomerPromotionCodesAsync(CustomerPromotionCode model)
        {
            var CustomerPromotionCodeToUpdate = await _context.CustomerPromotionCodes.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (CustomerPromotionCodeToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.IdCutomer != null) CustomerPromotionCodeToUpdate.IdCutomer = model.IdCutomer;
            if (model.IdPromotion != null) CustomerPromotionCodeToUpdate.IdPromotion = model.IdPromotion;
            if (model.IsUsed != null) CustomerPromotionCodeToUpdate.IsUsed = model.IsUsed;

            await _context.SaveChangesAsync();


            return new CustomerPromotionCode()
            {
                Id = model.Id,
                IdCutomer = model.IdCutomer,
                IdPromotion = model.IdPromotion,
                IsUsed = model.IsUsed
            };
        }


        public async Task<CustomerPromotionCode> AddCustomerPromotionCodesAsync(Domain.Models.CustomerPromotionCode model)
        {
            var newCustomerPromotionCode = new Dal.Entities.CustomerPromotionCode
            {
                IdCutomer = model.IdCutomer,
                IdPromotion = model.IdPromotion,
                IsUsed = model.IsUsed
            };

            _context.CustomerPromotionCodes.Add(newCustomerPromotionCode);
            _context.SaveChanges();

            return new CustomerPromotionCode()
            {
                Id = newCustomerPromotionCode.Id,
                IdPromotion = newCustomerPromotionCode.IdPromotion,
                IdCutomer = newCustomerPromotionCode.IdCutomer,
                IsUsed = newCustomerPromotionCode.IsUsed
            };
        }


        public async Task<bool> DeleteCustomerPromotionCodesAsync(int idCustomerPromotionCode)
        {
            var CustomerPromotionCodeToDelete = await _context.CustomerPromotionCodes.FirstOrDefaultAsync(u => u.Id == idCustomerPromotionCode);

            if (CustomerPromotionCodeToDelete == null)
                return false; // ou throw une exception;

            _context.CustomerPromotionCodes.Remove(CustomerPromotionCodeToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
