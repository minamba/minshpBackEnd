﻿using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Repositories
{
    public interface IVideoRepository
    {
        Task<IEnumerable<Video>> GetVideosAsync();
        Task<Video> UpdateVideosAsync(Video model);
        Task<Video> AddVideosAsync(Domain.Models.Video model);
        Task<bool> DeleteVideosAsync(int idVideo);

    }
}
