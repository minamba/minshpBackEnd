using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                Id_product = p.Id_product,
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
            if (model.Id_product != null) ImageToUpdate.Id_product = model.Id_product;

            await _context.SaveChangesAsync();


            return new Image()
            {
                Id = model.Id,
                Url = model.Url,
                Id_product = model.Id_product,
            };
        }


        public async Task<Image> AddImagesAsync(Domain.Models.Image model)
        {
            var newImage = new Dal.Entities.Image
            {
                Id = model.Id,
                Url = model.Url,
                Id_product = model.IdProduct
            };

            _context.Images.Add(newImage);
            _context.SaveChanges();

            return new Image()
            {
                Id = model.Id,
                Url = model.Url,
                Id_product = model.IdProduct
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
