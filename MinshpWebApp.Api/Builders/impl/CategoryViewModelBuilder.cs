using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;

namespace MinshpWebApp.Api.Builders.impl
{
    public class CategoryViewModelBuilder : ICategoryViewModelBuilder
    {
        private IMapper _mapper;
        private ICategoryService _categoryService;


        public CategoryViewModelBuilder(ICategoryService categoryService, IMapper mapper)
        {
            _mapper = mapper;
            _categoryService = categoryService;
        }

        public async Task<Category> AddCategorysAsync(CategoryRequest model)
        {
            var newCategory = _mapper.Map<Category>(model);
            return await _categoryService.AddCategorysAsync(newCategory);
        }

        public async Task<bool> DeleteCategorysAsync(int idCategory)
        {
            return await _categoryService.DeleteCategorysAsync(idCategory);
        }

        public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync()
        {
            var result = await _categoryService.GetCategoriesAsync();

            return  _mapper.Map<IEnumerable<CategoryViewModel>>(result);
        }

        public async Task<Category> UpdateCategorysAsync(CategoryRequest model)
        {
            return await _categoryService.UpdateCategorysAsync(_mapper.Map<Category>(model));
        }
    }
}
