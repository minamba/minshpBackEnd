using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IVideoViewModelBuilder
    {
        Task<IEnumerable<VideoViewModel>> GetVideosAsync();
        Task<Video> UpdateVideoAsync(VideoRequest model);
        Task<Video> AddVideoAsync(VideoRequest model);
        Task<bool> DeleteVideoAsync(int idImage);
    }
}
