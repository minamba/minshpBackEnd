using MinshpWebApp.Domain.Dtos;
using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface IProductFeatureService
    {
        Task<IEnumerable<ProductFeature>> GetProductFeaturesAsync();
        Task<ProductFeature> UpdateProductFeaturesAsync(ProductFeature model);
        Task<ProductFeature> AddProductFeaturesAsync(Domain.Models.ProductFeature model);
        Task<bool> DeleteProductFeaturesAsync(int idProduct);
    }
}
