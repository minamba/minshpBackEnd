using AutoMapper;
using MinshpWebApp.Domain.Dtos;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class ProductService : IProductService
    {
        private IMapper _mapper;
        private IProductRepository _repository;

        public ProductService(IProductRepository repository, IMapper mapper)
        {
            _repository= repository;
            _mapper= mapper;
        }

        public async Task<Product> AddProductsAsync(Product model)
        {
           return await _repository.AddProductsAsync(model);
        }

        public async Task<bool> DeleteProductsAsync(int idProduct)
        {
            return await _repository.DeleteProductsAsync(idProduct);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var products = await _repository.GetProductsAsync();
            var result = _mapper.Map<List<ProductDto>>(products);
            return result;
        }

        public async Task<IEnumerable<Product>> GetProductsByIdsAsync(IEnumerable<int> ids)
        {
            return await _repository.GetProductsByIdsAsync(ids);
        }

        public async Task<PageResult<int>> PageProductIdsAsync(PageRequest req, CancellationToken ct = default)
        {
            return await _repository.PageProductIdsAsync(req, ct);
        }

        public async Task<Product> UpdateProductsAsync(Product model)
        {
            return await _repository.UpdateProductsAsync(model);
        }
    }
}
