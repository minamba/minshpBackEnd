using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Video = MinshpWebApp.Domain.Models.Video;

namespace MinshpWebApp.Dal.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public VideoRepository()
        {
            _context = new MinshpDatabaseContext();
        }

        public async Task<IEnumerable<Video>> GetVideosAsync()
        {
            var VideoEntities = await _context.Videos.Select(p => new Video
            {
                Id = p.Id,
                Url = p.Url,
                IdProduct = p.Id_product,
            }).ToListAsync();

            return VideoEntities;
        }


        public async Task<Video> UpdateVideosAsync(Video model)
        {
            var VideoToUpdate = await _context.Videos.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (VideoToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.Url != null) VideoToUpdate.Url = model.Url;
            if (model.IdProduct != null) VideoToUpdate.Id_product = model.IdProduct;

            await _context.SaveChangesAsync();


            return new Video()
            {
                Id = model.Id,
                Url = model.Url,
                IdProduct = model.IdProduct,
            };
        }


        public async Task<Video> AddVideosAsync(Domain.Models.Video model)
        {
            var newVideo = new Dal.Entities.Video
            {
                Id = model.Id,
                Url = model.Url,
                Id_product = model.IdProduct
            };

            _context.Videos.Add(newVideo);
            _context.SaveChanges();

            return new Video()
            {
                Id = model.Id,
                Url = model.Url,
                IdProduct = model.IdProduct
            };
        }


        public async Task<bool> DeleteVideosAsync(int idVideo)
        {
            var VideoToDelete = await _context.Videos.FirstOrDefaultAsync(u => u.Id == idVideo);

            if (VideoToDelete == null)
                return false; // ou throw une exception;

            _context.Videos.Remove(VideoToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
