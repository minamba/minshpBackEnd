﻿using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Repositories
{
    public interface IFeatureRepository
    {
        Task<IEnumerable<Feature>> GetFeaturesAsync();
        Task<Feature> UpdateFeaturesAsync(Feature model);
        Task<Feature> AddFeaturesAsync(Domain.Models.Feature model);
        Task<bool> DeleteFeaturesAsync(int idFeature);
    }
}
