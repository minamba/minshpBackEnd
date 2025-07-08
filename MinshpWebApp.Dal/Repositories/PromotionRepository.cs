using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Dal.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public PromotionRepository()
        {
            _context = new MinshpDatabaseContext();
        }

        public async Task<IEnumerable<Promotion>> GetPromotionsAsync()
        {
            var PromotionEntities = await _context.Promotions.Select(p => new Promotion
            {
                Id = p.Id,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Purcentage = p.Purcentage,
                Id_product = p.Id_product,
            }).ToListAsync();

            return PromotionEntities;
        }


        public async Task<Promotion> UpdatePromotionsAsync(Promotion model)
        {
            var PromotionToUpdate = await _context.Promotions.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (PromotionToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.StartDate != null) PromotionToUpdate.StartDate = model.StartDate;
            if (model.EndDate != null) PromotionToUpdate.EndDate = model.EndDate;
            if (model.Purcentage != null) PromotionToUpdate.Purcentage = model.Purcentage;
            if (model.Id_product != null) PromotionToUpdate.Id_product = model.Id_product;

            await _context.SaveChangesAsync();


            return new Promotion()
            {
                Id = model.Id,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Purcentage = model.Purcentage,
                Id_product = model.Id_product,
            };
        }


        public async Task<Promotion> AddPromotionsAsync(Domain.Models.Promotion model)
        {
            var newPromotion = new Dal.Entities.Promotion
            {
                Id = model.Id,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Purcentage = model.Purcentage,
                Id_product = model.IdProduct
            };

            _context.Promotions.Add(newPromotion);
            _context.SaveChanges();

            return new Promotion()
            {
                Id = model.Id,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Purcentage = model.Purcentage,
                Id_product = model.IdProduct
            };
        }


        public async Task<bool> DeletePromotionsAsync(int idPromotion)
        {
            var PromotionToDelete = await _context.Promotions.FirstOrDefaultAsync(u => u.Id == idPromotion);

            if (PromotionToDelete == null)
                return false; // ou throw une exception;

            _context.Promotions.Remove(PromotionToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
