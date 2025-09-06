using AutoMapper;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class SubCategoryService : ISubCategoryService
    {
        private IMapper _mapper;
        private ISubCategoryRepository _repository;


        public SubCategoryService(ISubCategoryRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<SubCategory> AddSubCategorysAsync(SubCategory model)
        {
            return await _repository.AddSubCategorysAsync(model);
        }

        public async Task<bool> DeleteSubCategorysAsync(int idSubCategory)
        {
            return await _repository.DeleteSubCategorysAsync(idSubCategory);
        }

        public async Task<IEnumerable<SubCategory>> GetSubCategoriesAsync()
        {
            return await _repository.GetSubCategoriesAsync();
        }

        public async Task<SubCategory> UpdateSubCategorysAsync(SubCategory model)
        {
            return await _repository.UpdateSubCategorysAsync(model);
        }
    }
}
