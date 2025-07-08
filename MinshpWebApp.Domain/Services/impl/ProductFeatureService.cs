using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class ProductFeatureService : IProductFeatureService
    {

       IProductFeatureRepository _repository;


        public ProductFeatureService(IProductFeatureRepository repository)
        {
            _repository = repository;
        }


        public async Task<ProductFeature> AddProductFeaturesAsync(ProductFeature model)
        {
            return await _repository.AddProductFeaturesAsync(model);
        }

        public async Task<bool> DeleteProductFeaturesAsync(int idProduct)
        {
            return await _repository.DeleteProductFeaturesAsync(idProduct);
        }

        public async Task<IEnumerable<ProductFeature>> GetProductFeaturesAsync()
        {
            return await _repository.GetProductFeaturesAsync();
        }

        public async Task<ProductFeature> UpdateProductFeaturesAsync(ProductFeature model)
        {
            return await _repository.UpdateProductFeaturesAsync(model);
        }
    }
}
