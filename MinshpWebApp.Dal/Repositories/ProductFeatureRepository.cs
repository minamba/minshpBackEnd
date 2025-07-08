using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductFeature = MinshpWebApp.Domain.Models.ProductFeature;

namespace MinshpWebApp.Dal.Repositories
{
    public class ProductFeatureRepository : IProductFeatureRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public ProductFeatureRepository()
        {
            _context = new MinshpDatabaseContext();
        }

        public async Task<IEnumerable<ProductFeature>> GetProductFeaturesAsync()
        {
            var ProductFeatureEntities = await _context.ProductFeatures.Select(p => new ProductFeature
            {
                Id = p.Id,
                IdProduct = p.IdProduct,
                IdFeature = p.Id_feature,
            }).ToListAsync();

            return ProductFeatureEntities;
        }


        public async Task<ProductFeature> UpdateProductFeaturesAsync(ProductFeature model)
        {
            var ProductFeatureToUpdate = await _context.ProductFeatures.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (ProductFeatureToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.IdProduct != null) ProductFeatureToUpdate.IdProduct = model.IdProduct;
            if (model.IdFeature != null) ProductFeatureToUpdate.Id_feature = model.IdFeature;

            await _context.SaveChangesAsync();


            return new ProductFeature()
            {
                Id = model.Id,
                IdProduct = model.IdProduct,
                IdFeature = model.IdFeature,
            };
        }


        public async Task<ProductFeature> AddProductFeaturesAsync(Domain.Models.ProductFeature model)
        {
            var newProductFeature = new Dal.Entities.ProductFeature
            {
                Id = model.Id,
                IdProduct = model.IdProduct,
                Id_feature = model.IdFeature,
            };

            _context.ProductFeatures.Add(newProductFeature);
            _context.SaveChanges();

            return new ProductFeature()
            {
                Id = model.Id,
                IdProduct = model.IdProduct,
                IdFeature = model.IdFeature,
            };
        }


        public async Task<bool> DeleteProductFeaturesAsync(int idProductFeature)
        {
            var ProductFeatureToDelete = await _context.ProductFeatures.FirstOrDefaultAsync(u => u.Id == idProductFeature);

            if (ProductFeatureToDelete == null)
                return false; // ou throw une exception;

            _context.ProductFeatures.Remove(ProductFeatureToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
