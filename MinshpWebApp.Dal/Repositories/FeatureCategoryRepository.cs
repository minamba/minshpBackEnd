using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureCategory = MinshpWebApp.Domain.Models.FeatureCategory;

namespace MinshpWebApp.Dal.Repositories
{
    public class FeatureCategoryRepository : IFeatureCategoryRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public FeatureCategoryRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FeatureCategory>> GetFeatureCategoriesAsync()
        {
            var FeatureCategoryEntities = await _context.FeatureCategories.Select(p => new FeatureCategory
            {
                Id = p.Id,
                Name = p.Name,
            }).ToListAsync();

            return FeatureCategoryEntities;
        }


        public async Task<FeatureCategory> UpdateFeatureCategoryAsync(FeatureCategory model)
        {
            var FeatureCategoryToUpdate = await _context.FeatureCategories.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (FeatureCategoryToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.Name != null) FeatureCategoryToUpdate.Name = model.Name;

            await _context.SaveChangesAsync();


            return new FeatureCategory()
            {
                Id = model.Id,
                Name = model.Name,
            };
        }


        public async Task<FeatureCategory> AddFeatureCategoryAsync(FeatureCategory model)
        {
            var newFeatureCategory = new Dal.Entities.FeatureCategory
            {
                Id = model.Id,
                Name = model.Name,
            };

            _context.FeatureCategories.Add(newFeatureCategory);
            _context.SaveChanges();

            return new FeatureCategory()
            {
                Id = model.Id,
                Name = model.Name,
            };
        }


        public async Task<bool> DeleteFeatureCategoryAsync(int idFeatureCategory)
        {
            var FeatureCategoryToDelete = await _context.FeatureCategories.FirstOrDefaultAsync(u => u.Id == idFeatureCategory);

            if (FeatureCategoryToDelete == null)
                return false; // ou throw une exception;

            _context.FeatureCategories.Remove(FeatureCategoryToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
