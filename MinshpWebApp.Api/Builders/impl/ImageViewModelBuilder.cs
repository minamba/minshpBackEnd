using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;

namespace MinshpWebApp.Api.Builders.impl
{
    public class ImageViewModelBuilder : IImageViewModelBuilder
    {
        private IMapper _mapper;
        private IImageService _imageService;


        public ImageViewModelBuilder(IImageService imageService, IMapper mapper)
        {
            _imageService = imageService;
            _mapper = mapper;
        }

        public async Task<Image> AddImagesAsync(ImageRequest model)
        {
            var newImage = _mapper.Map<Image>(model);
            return await _imageService.AddImagesAsync(newImage);
        }

        public async Task<bool> DeleteImagesAsync(int idImage)
        {
           return await _imageService.DeleteImagesAsync(idImage);
        }

        public async Task<IEnumerable<ImageViewModel>> GetImagesAsync()
        {
            var images = await _imageService.GetImagesAsync();

            return _mapper.Map<IEnumerable<ImageViewModel>>(images);
        }

        public async Task<Image> UpdateImagesAsync(ImageRequest model)
        {

            var imageToGetForResetOldImage = (await _imageService.GetImagesAsync()).FirstOrDefault(c => c.IdCategory == model.IdCategory);
            imageToGetForResetOldImage.IdCategory = null;
            await _imageService.UpdateImagesAsync(imageToGetForResetOldImage);


            var image = _mapper.Map<Image>(model);



            return await _imageService.UpdateImagesAsync(image);
        }
    }
}
