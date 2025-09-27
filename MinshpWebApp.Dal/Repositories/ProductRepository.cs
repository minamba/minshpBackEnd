using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Dal.Utils;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Product = MinshpWebApp.Domain.Models.Product;

namespace MinshpWebApp.Dal.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private MinshpDatabaseContext _context { get; set; }

        public ProductRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var productEntities = await _context.Products
                .OrderByDescending(p => p.CreationDate)
                .Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                IdCategory = p.Id_Category,
                Price = p.Price,
                Main = p.Main,
                Brand = p.Brand,
                Model = p.Model,
                CreationDate = p.CreationDate,
                ModificationDate = p.ModificationDate,
                IdPromotionCode = p.IdPromotionCode,
                IdPackageProfil = p.IdPackageProfil,
                IdSubCategory = p.IdSubCategory,
                Display = p.Display

                }).ToListAsync();

            return productEntities;
        }


        public async Task<Product> UpdateProductsAsync(Product model)
        {
            var ProductToUpdate = await _context.Products.FirstOrDefaultAsync(u => u.Id == model.Id  );
            var StockToUpdate = await _context.Stocks.FirstOrDefaultAsync(s => s.IdProduct == model.Id);
            var GetPromotion = await _context.PromotionCodes.FirstOrDefaultAsync(s => s.Id == model.IdPromotionCode);

            if (ProductToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.Description != null) ProductToUpdate.Description = model.Description;
            if (model.Price != null) ProductToUpdate.Price = model.Price;
            if (model.Name != null) ProductToUpdate.Name = model.Name;
            if (model.Main != null) ProductToUpdate.Main = model.Main;
            if (model.IdCategory != null) ProductToUpdate.Id_Category = model.IdCategory;
            if (model.Brand != null) ProductToUpdate.Brand = model.Brand;
            if (model.Model != null) ProductToUpdate.Model = model.Model;
            if (model.ModificationDate != null) ProductToUpdate.ModificationDate = model.ModificationDate;
            if (model.Display != null) ProductToUpdate.Display = model.Display;
            ProductToUpdate.IdPackageProfil = model.IdPackageProfil;
            ProductToUpdate.IdSubCategory = model.IdSubCategory;


            if (GetPromotion != null)
            {
                if (model.IdPromotionCode != null) ProductToUpdate.IdPromotionCode = model.IdPromotionCode;
            }
            else
                ProductToUpdate.IdPromotionCode = null;

             
            await _context.SaveChangesAsync();


            if (model.Stock != null) StockToUpdate.Quantity = model.Stock;

            await _context.SaveChangesAsync();


            return new Product()
            { 
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                IdCategory= model.IdCategory,
                Stock = model.Stock,
                Main = model.Main,
                Brand = model.Brand,
                Model = model.Model,
                ModificationDate = model.ModificationDate,
                CreationDate = model.CreationDate,
                IdPromotionCode = model.IdPromotionCode,
                IdPackageProfil = model.IdPackageProfil,
                IdSubCategory = model.IdSubCategory,
                Display = model.Display
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
                Id_Category = model.IdCategory,
                Brand = model.Brand,
                Model = model.Model,
                CreationDate= DateTime.Now,
                IdPromotionCode = model.IdPromotionCode,
                IdPackageProfil = model.IdPackageProfil,
                IdSubCategory = model.IdSubCategory,
                Display = model.Display
            };

            _context.Products.Add(newProduct);
            _context.SaveChanges();


            //insertion dans la table stock 
            var newStock = new Dal.Entities.Stock
            {
                IdProduct = newProduct.Id, 
                Quantity = model.Stock
            };

            // 4️⃣ Ajouter à la base
            _context.Stocks.Add(newStock);
            await _context.SaveChangesAsync();


            return new Product() 
            {
                Id = newStock.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                IdCategory = model.IdCategory,
                Brand = newProduct.Brand,
                Model = model.Model,
                CreationDate= DateTime.Now,
                ModificationDate= null,
                IdPromotionCode = model.IdPromotionCode,
                IdPackageProfil = model.IdPackageProfil,
                IdSubCategory = model.IdSubCategory,
                Display = model.Display
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



        // cette methode est a cleaner car elle ne sert plus du tout. tu pourras l'enlever de partout
        public async Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids.Distinct().ToList();
            var productEntities = await _context.Products
                .AsNoTracking()
                .Where(p => idList.Contains(p.Id))
                .OrderByDescending(p => p.CreationDate)
                .Select(p => new Product
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    IdCategory = p.Id_Category,
                    Price = p.Price,
                    Main = p.Main,
                    Brand = p.Brand,
                    Model = p.Model,
                    CreationDate = p.CreationDate,
                    ModificationDate = p.ModificationDate,
                    IdPromotionCode = p.IdPromotionCode,
                    IdPackageProfil = p.IdPackageProfil,
                    IdSubCategory = p.IdSubCategory,
                    Display = p.Display
                })
                .ToListAsync();

            // Remet l'ordre des IDs paginés (important pour garder le tri)
            var order = idList.Select((id, i) => new { id, i }).ToDictionary(x => x.id, x => x.i);
            return productEntities.OrderBy(p => order[p.Id]).ToList();
        }



        public async Task<PageResult<int>> PageProductIdsAsync(PageRequest req, CancellationToken ct = default)
        {
            var q = _context.Products.AsNoTracking();

            // champs de recherche génériques
            var search = new Expression<Func<Dal.Entities.Product, string?>>[]
            {
        p => p.Name, p => p.Description, p => p.Brand, p => p.Model, p => p.Main.ToString()
            };

            // filtres génériques (clé = Filter.<Key> côté front)
            var filters = new Dictionary<string, Func<IQueryable<Dal.Entities.Product>, string, IQueryable<Dal.Entities.Product>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["IdCategory"] = (qq, v) => int.TryParse(v, out var id) ? qq.Where(p => p.Id_Category == id) : qq,
                ["IdSubCategory"] = (qq, v) => int.TryParse(v, out var id) ? qq.Where(p => p.IdSubCategory == id) : qq,
                ["Main"] = (qq, v) => bool.TryParse(v, out var b) ? qq.Where(p => p.Main == b) : qq,
                ["Brand"] = (qq, v) => qq.Where(p => p.Brand == v),
                ["Model"] = (qq, v) => qq.Where(p => p.Model == v),
                ["PriceMin"] = (qq, v) => decimal.TryParse(v, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var min) ? qq.Where(p => p.Price >= min) : qq,
                ["PriceMax"] = (qq, v) => decimal.TryParse(v, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var max) ? qq.Where(p => p.Price <= max) : qq,
            };

            // On page d'abord sur les IDs (rapide + stable), tri par défaut
            var page = await PagedQuery.ExecuteAsync<Dal.Entities.Product, int>(
                query: q,
                req: req,
                searchFields: search,
                filterHandlers: filters,
                defaultSort: "CreationDate:desc,Name:asc",
                selector: p => p.Id,
                ct: ct
            );

            return page;
        }
    }
}
