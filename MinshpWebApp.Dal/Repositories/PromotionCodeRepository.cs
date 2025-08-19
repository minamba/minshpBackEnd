using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PromotionCode = MinshpWebApp.Domain.Models.PromotionCode;

namespace MinshpWebApp.Dal.Repositories
{
    public class PromotionCodeRepository : IPromotionCodeRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public PromotionCodeRepository()
        {
            _context = new MinshpDatabaseContext();
        }

        public async Task<IEnumerable<PromotionCode>> GetPromotionCodesAsync()
        {
            var PromotionCodeEntities = await _context.PromotionCodes.Select(p => new PromotionCode
            {
                Id = p.Id,
                Name = p.Name,
                DateCreation = p.DateCreation,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Purcentage = p.Purcentage,
                IsUsed = p.IsUsed,
            }).ToListAsync();

            return PromotionCodeEntities;
        }


        public async Task<PromotionCode> UpdatePromotionCodesAsync(PromotionCode model)
        {
            var PromotionCodeToUpdate = await _context.PromotionCodes.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (PromotionCodeToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.Name != null) PromotionCodeToUpdate.Name = model.Name;
            if (model.StartDate != null) PromotionCodeToUpdate.StartDate = model.StartDate;
            if (model.EndDate != null) PromotionCodeToUpdate.EndDate = model.EndDate;
            if (model.Purcentage != null) PromotionCodeToUpdate.Purcentage = model.Purcentage;
            if (model.IsUsed != null) PromotionCodeToUpdate.IsUsed = model.IsUsed;

            await _context.SaveChangesAsync();


            return new PromotionCode()
            {
                Id = model.Id,
                Name = model.Name,
                DateCreation = model.DateCreation,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Purcentage = model.Purcentage,
                IsUsed = model.IsUsed,
            };
        }


        public async Task<PromotionCode> AddPromotionCodesAsync(Domain.Models.PromotionCode model)
        {
            var newPromotionCode = new Dal.Entities.PromotionCode
            {
                Name = model.Name,
                DateCreation = DateTime.Now,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Purcentage = model.Purcentage,
                IsUsed = model.IsUsed,
            };

            _context.PromotionCodes.Add(newPromotionCode);
            _context.SaveChanges();

            return new PromotionCode()
            {
                Id = newPromotionCode.Id,
                Name = newPromotionCode.Name,
                DateCreation = newPromotionCode.DateCreation,
                StartDate = newPromotionCode.StartDate,
                EndDate = newPromotionCode.EndDate,
                Purcentage = newPromotionCode.Purcentage,
                IsUsed = newPromotionCode.IsUsed,
            };
        }


        public async Task<bool> DeletePromotionCodesAsync(int idPromotionCode)
        {
            var PromotionCodeToDelete = await _context.PromotionCodes.FirstOrDefaultAsync(u => u.Id == idPromotionCode);

            if (PromotionCodeToDelete == null)
                return false; // ou throw une exception;

            _context.PromotionCodes.Remove(PromotionCodeToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
