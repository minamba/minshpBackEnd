using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface ISubCategoryService
    {
        Task<IEnumerable<SubCategory>> GetSubCategoriesAsync();
        Task<SubCategory> UpdateSubCategorysAsync(SubCategory model);
        Task<SubCategory> AddSubCategorysAsync(Domain.Models.SubCategory model);
        Task<bool> DeleteSubCategorysAsync(int idSubCategory);
    }
}
