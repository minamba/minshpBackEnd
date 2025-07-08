using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class ImageService : IImageService
    {

        IImageRepository _repository;


        public ImageService(IImageRepository repository)
        {
            _repository = repository;
        }

        public async Task<Image> AddImagesAsync(Image model)
        {
            return await _repository.AddImagesAsync(model);
        }

        public async Task<bool> DeleteImagesAsync(int idImage)
        {
            return await _repository.DeleteImagesAsync(idImage);
        }

        public async Task<IEnumerable<Image>> GetImagesAsync()
        {
           return await _repository.GetImagesAsync(); ;
        }

        public async Task<Image> UpdateImagesAsync(Image model)
        {
           return await _repository.UpdateImagesAsync(model);
        }
    }
}
