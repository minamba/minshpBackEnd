using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Feature = MinshpWebApp.Domain.Models.Feature;

namespace MinshpWebApp.Dal.Repositories
{
    public class FeatureRepository : IFeatureRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public FeatureRepository()
        {
            _context = new MinshpDatabaseContext();
        }

        public async Task<IEnumerable<Feature>> GetFeaturesAsync()
        {
            var FeatureEntities = await _context.Features.Select(p => new Feature
            {
                Id = p.Id,
                Description = p.Description,
                IdCategory = p.IdCategory,
                IdFeatureCategory = p.IdFeatureCategory,
            }).ToListAsync();

            return FeatureEntities;
        }


        public async Task<Feature> UpdateFeaturesAsync(Feature model)
        {
            var FeatureToUpdate = await _context.Features.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (FeatureToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.Description != null) FeatureToUpdate.Description = model.Description;
            if (model.IdCategory != null) FeatureToUpdate.IdCategory = model.IdCategory;
            if (model.IdFeatureCategory != null) FeatureToUpdate.IdFeatureCategory = model.IdFeatureCategory;

            await _context.SaveChangesAsync();


            return new Feature()
            {
                Id = model.Id,
                Description = model.Description,
                IdCategory = model.IdCategory,
                IdFeatureCategory = model.IdFeatureCategory,
            };
        }


        public async Task<Feature> AddFeaturesAsync(Domain.Models.Feature model)
        {
            var newFeature = new Dal.Entities.Feature
            {
                Id = model.Id,
                Description = model.Description,
                IdCategory = model.IdCategory,
                IdFeatureCategory = model.IdFeatureCategory,
            };

            _context.Features.Add(newFeature);
            _context.SaveChanges();

            return new Feature()
            {
                Id = model.Id,
                Description = model.Description,
                IdCategory = model.IdCategory,
                IdFeatureCategory = model.IdFeatureCategory,
            };
        }


        public async Task<bool> DeleteFeaturesAsync(int idFeature)
        {
            var FeatureToDelete = await _context.Features.FirstOrDefaultAsync(u => u.Id == idFeature);

            if (FeatureToDelete == null)
                return false; // ou throw une exception;

            _context.Features.Remove(FeatureToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
