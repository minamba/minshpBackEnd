using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IImageViewModelBuilder
    {
        Task<IEnumerable<ImageViewModel>> GetImagesAsync();
        Task<Image> UpdateImagesAsync(ImageRequest model);
        Task<Image> AddImagesAsync(ImageRequest model);
        Task<bool> DeleteImagesAsync(int idImage);
    }
}
