using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface ICategoryViewModelBuilder
    {
        Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync();
        Task<Category> UpdateCategorysAsync(CategoryRequest model);
        Task<Category> AddCategorysAsync(CategoryRequest model);
        Task<bool> DeleteCategorysAsync(int idCategory);
    }
}
