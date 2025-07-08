using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class CategoryService : ICategoryService
    {

        ICategoryRepository _repository;


        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }


        public async Task<Category> AddCategorysAsync(Category model)
        {
           return await _repository.AddCategorysAsync(model);
        }

        public async Task<bool> DeleteCategorysAsync(int idCategory)
        {
            return await _repository.DeleteCategorysAsync(idCategory);
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _repository.GetCategoriesAsync();
        }

        public async Task<Category> UpdateCategorysAsync(Category model)
        {
            return await _repository.UpdateCategorysAsync(model);
        }
    }
}
