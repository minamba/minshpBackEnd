using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface ISubCategoryViewModelBuilder
    {
        Task<IEnumerable<SubCategoryViewModel>> GetSubCategoriesAsync();
        Task<SubCategory> UpdateSubCategorysAsync(SubCategoryRequest model);
        Task<SubCategory> AddSubCategorysAsync(SubCategoryRequest model);
        Task<bool> DeleteSubCategorysAsync(int idSubCategory);
    }
}
