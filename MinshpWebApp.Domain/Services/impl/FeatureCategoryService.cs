using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class FeatureCategoryService : IFeatureCategoryService
    {
        IFeatureCategoryRepository _repository;


        public FeatureCategoryService(IFeatureCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<FeatureCategory> AddFeatureCategoryAsync(FeatureCategory model)
        {
            return await _repository.AddFeatureCategoryAsync(model);
        }

        public async Task<bool> DeleteFeatureCategoryAsync(int idFeatureCategory)
        {
            return await _repository.DeleteFeatureCategoryAsync(idFeatureCategory);
        }

        public async Task<IEnumerable<FeatureCategory>> GetFeatureCategoriesAsync()
        {
            return await _repository.GetFeatureCategoriesAsync();
        }

        public async Task<FeatureCategory> UpdateFeatureCategoryAsync(FeatureCategory model)
        {
           return await _repository.UpdateFeatureCategoryAsync(model);
        }
    }
}
