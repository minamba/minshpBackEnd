using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubCategory = MinshpWebApp.Domain.Models.SubCategory;

namespace MinshpWebApp.Dal.Repositories
{
    public class SubCategoryRepository : ISubCategoryRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public SubCategoryRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubCategory>> GetSubCategoriesAsync()
        {
            var SubCategoryEntities = await _context.SubCategories.Select(p => new SubCategory
            {
                Id = p.Id,
                Name = p.Name,
                IdTaxe = p.IdTaxe,
                IdPromotionCode = p.IdPromotionCode,
                ContentCode = p.ContentCode,
                IdCategory = p.IdCategory,
                Display = p.Display
            }).ToListAsync();

            return SubCategoryEntities;
        }


        public async Task<SubCategory> UpdateSubCategorysAsync(SubCategory model)
        {
            var SubCategoryToUpdate = await _context.SubCategories.FirstOrDefaultAsync(u => u.Id == model.Id);
            var GetPromotion = await _context.PromotionCodes.FirstOrDefaultAsync(s => s.Id == model.IdPromotionCode);


            if (SubCategoryToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.Name != null) SubCategoryToUpdate.Name = model.Name;
            if (model.IdTaxe != null) SubCategoryToUpdate.IdTaxe = model.IdTaxe;
            if (model.ContentCode != null) SubCategoryToUpdate.ContentCode = model.ContentCode;
            if (model.IdCategory != null) SubCategoryToUpdate.IdCategory = model.IdCategory;
            if (model.Display != null) SubCategoryToUpdate.Display = model.Display;


            if (GetPromotion != null)
            {
                if (model.IdPromotionCode != null) SubCategoryToUpdate.IdPromotionCode = model.IdPromotionCode;
            }
            else
                SubCategoryToUpdate.IdPromotionCode = null;


            await _context.SaveChangesAsync();


            return new SubCategory()
            {
                Id = model.Id,
                Name = model.Name,
                IdTaxe = model.IdTaxe,
                IdPromotionCode = model.IdPromotionCode,
                IdCategory = model.IdCategory,
                ContentCode = model.ContentCode,
                Display = model.Display
            };
        }


        public async Task<SubCategory> AddSubCategorysAsync(Domain.Models.SubCategory model)
        {
            var newSubCategory = new Dal.Entities.SubCategory
            {
                Id = model.Id,
                Name = model.Name,
                IdTaxe = model.IdTaxe,
                IdPromotionCode = model.IdPromotionCode,
                IdCategory = model.IdCategory,
                ContentCode = model.ContentCode,
                Display = model.Display
            };

            _context.SubCategories.Add(newSubCategory);
            _context.SaveChanges();

            return new SubCategory()
            {
                Id = model.Id,
                Name = model.Name,
                IdTaxe = model.IdTaxe,
                IdPromotionCode = model.IdPromotionCode,
                IdCategory = model.IdCategory,
                ContentCode = model.ContentCode,
                Display = model.Display
            };
        }


        public async Task<bool> DeleteSubCategorysAsync(int idSubCategory)
        {
            var SubCategoryToDelete = await _context.SubCategories.FirstOrDefaultAsync(u => u.Id == idSubCategory);

            if (SubCategoryToDelete == null)
                return false; // ou throw une exception;

            _context.SubCategories.Remove(SubCategoryToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
