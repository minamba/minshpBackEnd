﻿using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> UpdateCategorysAsync(Category model);
        Task<Category> AddCategorysAsync(Domain.Models.Category model);
        Task<bool> DeleteCategorysAsync(int idCategory);
    }
}
