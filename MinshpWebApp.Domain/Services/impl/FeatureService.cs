using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class FeatureService : IFeatureService
    {
        IFeatureRepository _repository;


        public FeatureService(IFeatureRepository repository)
        {
            _repository = repository;
        }

        public async Task<Feature> AddFeaturesAsync(Feature model)
        {
            return await _repository.AddFeaturesAsync(model);
        }

        public async Task<bool> DeleteFeaturesAsync(int idFeature)
        {
            return await _repository.DeleteFeaturesAsync(idFeature);    
        }

        public async Task<IEnumerable<Feature>> GetFeaturesAsync()
        {
            return await _repository.GetFeaturesAsync();
        }

        public async Task<Feature> UpdateFeaturesAsync(Feature model)
        {
            return await _repository.UpdateFeaturesAsync(model);
        }
    }
}
