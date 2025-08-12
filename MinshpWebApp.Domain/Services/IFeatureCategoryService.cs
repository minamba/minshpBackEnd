using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface IFeatureCategoryService
    {
        Task<IEnumerable<FeatureCategory>> GetFeatureCategoriesAsync();
        Task<FeatureCategory> UpdateFeatureCategoryAsync(FeatureCategory model);
        Task<FeatureCategory> AddFeatureCategoryAsync(Domain.Models.FeatureCategory model);
        Task<bool> DeleteFeatureCategoryAsync(int idFeatureCategory);
    }
}
