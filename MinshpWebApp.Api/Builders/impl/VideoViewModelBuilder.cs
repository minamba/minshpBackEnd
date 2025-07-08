using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;

namespace MinshpWebApp.Api.Builders.impl
{
    public class VideoViewModelBuilder : IVideoViewModelBuilder
    {
        private IMapper _mapper;
        private IVideoService _VideoService;


        public VideoViewModelBuilder(IVideoService VideoService, IMapper mapper)
        {
            _VideoService = VideoService;
            _mapper = mapper;
        }

        public async Task<Video> AddVideoAsync(VideoRequest model)
        {
            var newVideo = _mapper.Map<Video>(model);
            return await _VideoService.AddVideosAsync(newVideo);
        }


        public async Task<bool> DeleteVideoAsync(int idVideo)
        {
            return await _VideoService.DeleteVideosAsync(idVideo);
        }

 
        public async Task<IEnumerable<VideoViewModel>> GetVideosAsync()
        {
            var Videos = await _VideoService.GetVideosAsync();

            return _mapper.Map<IEnumerable<VideoViewModel>>(Videos);
        }

        public async Task<Video> UpdateVideoAsync(VideoRequest model)
        {
            var Video = _mapper.Map<Video>(model);

            return await _VideoService.UpdateVideosAsync(Video);
        }
    }
}
