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

        public CategoryRepository()
        {
            _context = new MinshpDatabaseContext();
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var CategoryEntities = await _context.Categories.Select(p => new Category
            {
                Id = p.Id,
                Name = p.Name,
                IdTaxe = p.IdTaxe,
                IdPromotionCode = p.IdPromotionCode,
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


            if (GetPromotion != null)
            {
                if (model.IdPromotionCode != null) CategoryToUpdate.IdPromotionCode = model.IdPromotionCode;
            }
            else
                CategoryToUpdate.IdPromotionCode = null;


            await _context.SaveChangesAsync();


            return new Category()
            {
                Id = model.Id,
                Name = model.Name,
                IdTaxe = model.IdTaxe,
                IdPromotionCode = model.IdPromotionCode,
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
            };

            _context.Categories.Add(newCategory);
            _context.SaveChanges();

            return new Category()
            {
                Id = model.Id,
                Name = model.Name,
                IdTaxe = model.IdTaxe,
                IdPromotionCode = model.IdPromotionCode,
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
