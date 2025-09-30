using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Category = MinshpWebApp.Domain.Models.Category;

namespace MinshpWebApp.Dal.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public CategoryRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var CategoryEntities = await _context.Categories.Select(p => new Category
            {
                Id = p.Id,
                Name = p.Name,
                IdTaxe = p.IdTaxe,
                IdPromotionCode = p.IdPromotionCode,
                IdPackageProfil = p.IdPackageProfil,
                ContentCode = p.ContentCode,
                Display = p.Display
            }).ToListAsync();

            return CategoryEntities;
        }


        public async Task<Category> UpdateCategorysAsync(Category model)
        {
            var CategoryToUpdate = await _context.Categories.FirstOrDefaultAsync(u => u.Id == model.Id);
            var GetPromotion = await _context.PromotionCodes.FirstOrDefaultAsync(s => s.Id == model.IdPromotionCode);


            if (CategoryToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.Name != null) CategoryToUpdate.Name = model.Name;
            if (model.IdTaxe != null) CategoryToUpdate.IdTaxe = model.IdTaxe;
            if (model.IdPackageProfil != null) CategoryToUpdate.IdPackageProfil = model.IdPackageProfil;
            if (model.ContentCode != null) CategoryToUpdate.ContentCode = model.ContentCode;
            if (model.Display != null) CategoryToUpdate.Display = model.Display;


            if (GetPromotion != null)
            {
                if (model.IdPromotionCode != null) CategoryToUpdate.IdPromotionCode = model.IdPromotionCode;
            }


            await _context.SaveChangesAsync();


            return new Category()
            {
                Id = model.Id,
                Name = model.Name,
                IdTaxe = model.IdTaxe,
                IdPromotionCode = model.IdPromotionCode,
                IdPackageProfil = model.IdPackageProfil,
                ContentCode = model.ContentCode,
                Display = model.Display
            };
        }


        public async Task<Category> AddCategorysAsync(Domain.Models.Category model)
        {
            var newCategory = new Dal.Entities.Category
            {
                Id = model.Id,
                Name = model.Name,
                IdTaxe = model.IdTaxe,
                IdPromotionCode = model.IdPromotionCode,
                IdPackageProfil = model.IdPackageProfil,
                ContentCode = model.ContentCode,
                Display = model.Display
            };

            _context.Categories.Add(newCategory);
            _context.SaveChanges();

            return new Category()
            {
                Id = model.Id,
                Name = model.Name,
                IdTaxe = model.IdTaxe,
                IdPromotionCode = model.IdPromotionCode,
                IdPackageProfil = model.IdPackageProfil,
                ContentCode = model.ContentCode,
                Display = model.Display
            };
        }


        public async Task<bool> DeleteCategorysAsync(int idCategory)
        {
            var CategoryToDelete = await _context.Categories.FirstOrDefaultAsync(u => u.Id == idCategory);

            if (CategoryToDelete == null)
                return false; // ou throw une exception;

            _context.Categories.Remove(CategoryToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
