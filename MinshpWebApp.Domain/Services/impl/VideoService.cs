using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class VideoService : IVideoService
    {
        IVideoRepository _repository;


        public VideoService(IVideoRepository repository)
        {
            _repository = repository;
        }


        public async Task<Video> AddVideosAsync(Video model)
        {
            return await _repository.AddVideosAsync(model);
        }

        public async Task<bool> DeleteVideosAsync(int idVideo)
        {
           return await _repository.DeleteVideosAsync(idVideo);
        }

        public async Task<IEnumerable<Video>> GetVideosAsync()
        {
            return await _repository.GetVideosAsync();
        }

        public async Task<Video> UpdateVideosAsync(Video model)
        {
           return await _repository.UpdateVideosAsync(model);
        }
    }
}
