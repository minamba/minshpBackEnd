using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using Product = MinshpWebApp.Domain.Models.Product;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MinshpWebApp.Dal.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private MinshpDatabaseContext _context { get; set; }

        public ProductRepository()
        {
            _context = new MinshpDatabaseContext();
        }


        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var productEntities = await _context.Products.Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                IdCategory = p.Id_Category,
                Price = p.Price
            }).ToListAsync();

            return productEntities;
        }


        public async Task<Product> UpdateProductsAsync(Product model)
        {
            var ProductToUpdate = await _context.Products.FirstOrDefaultAsync(u => u.Id == model.Id  );

            if (ProductToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.Description != null) ProductToUpdate.Description = model.Description;
            if (model.Price != null) ProductToUpdate.Price = model.Price;
            if (model.Name != null) ProductToUpdate.Name = model.Name;
            if (model.IdCategory != null) ProductToUpdate.Id_Category = model.IdCategory;

            await _context.SaveChangesAsync();


            return new Product()
            { 
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                IdCategory= model.IdCategory,
            };
        }


        public async Task<Product> AddProductsAsync(Domain.Models.Product model)
        {
            var newProduct = new Dal.Entities.Product
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,

            };

            _context.Products.Add(newProduct);
            _context.SaveChanges();

            return new Product() 
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                IdCategory = model.IdCategory,
            };
        }


        public async Task<bool> DeleteProductsAsync( int idProduct)
        {
            var productToDelete = await _context.Products.FirstOrDefaultAsync(u => u.Id == idProduct);

            if (productToDelete == null)
                return false; // ou throw une exception;

            _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
