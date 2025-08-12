using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Image = MinshpWebApp.Domain.Models.Image;

namespace MinshpWebApp.Dal.Repositories
{
    public  class ImageRepository : IImageRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public ImageRepository()
        {
            _context = new MinshpDatabaseContext();
        }

        public async Task<IEnumerable<Image>> GetImagesAsync()
        {
            var ImageEntities = await _context.Images.Select(p => new Image
            {
                Id = p.Id,
                Url = p.Url,
                Description = p.Description,
                Title = p.Title,
                IdProduct = p.Id_product,
                Position = p.Position,
                IdCategory = p.IdCategory,
            }).ToListAsync();

            return ImageEntities;
        }


        public async Task<Image> UpdateImagesAsync(Image model)
        {
            var ImageToUpdate = await _context.Images.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (ImageToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.Url != null) ImageToUpdate.Url = model.Url;
            if (model.Description != null) ImageToUpdate.Description = model.Description;
            if (model.Title != null) ImageToUpdate.Title = model.Title;
            if (model.IdProduct != null) ImageToUpdate.Id_product = model.IdProduct;
            if (model.Position != null) ImageToUpdate.Position = model.Position;
            if (model.IdCategory != null) ImageToUpdate.IdCategory = model.IdCategory;

            await _context.SaveChangesAsync();


            return new Image()
            {
                Id = model.Id,
                Url = model.Url,
                Description = model.Description,
                Title = model.Title,
                IdProduct = model.IdProduct,
                Position = model.Position,
                IdCategory = model.IdCategory,
            };
        }


        public async Task<Image> AddImagesAsync(Domain.Models.Image model)
        {
            var newImage = new Dal.Entities.Image
            {
                Id = model.Id,
                Url = model.Url,
                Id_product = model.IdProduct,
                Description = model.Description,
                Title = model.Title,
                Position = model.Position,
                IdCategory = model.IdCategory,
            };

            _context.Images.Add(newImage);
            _context.SaveChanges();

            return new Image()
            {
                Id = model.Id,
                Url = model.Url,
                IdProduct = model.IdProduct,
                Description = model.Description,
                Title = model.Title,
                Position = model.Position,
                IdCategory = model.IdCategory,
            };
        }


        public async Task<bool> DeleteImagesAsync(int idImage)
        {
            var ImageToDelete = await _context.Images.FirstOrDefaultAsync(u => u.Id == idImage);

            if (ImageToDelete == null)
                return false; // ou throw une exception;

            _context.Images.Remove(ImageToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
