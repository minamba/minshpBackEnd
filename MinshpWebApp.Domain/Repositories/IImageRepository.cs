﻿using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Repositories
{
    public interface IImageRepository
    {
        Task<IEnumerable<Image>> GetImagesAsync();
        Task<Image> UpdateImagesAsync(Image model);
        Task<Image> AddImagesAsync(Domain.Models.Image model);
        Task<bool> DeleteImagesAsync(int idImage);
    }
}
