using AutoMapper;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Services;

namespace MinshpWebApp.Api.Builders.impl
{
    public class ProductViewModelBuilder : IProductViewModelBuilder
    {
        private IMapper _mapper;
        private IProductService _productService;


        public ProductViewModelBuilder(IProductService productService, IMapper mapper)
        {
            _mapper = mapper;
            _productService = productService;
        }

        public async Task<IEnumerable<ProductVIewModel>> GetProductsAsync()
        {
            var products = await _productService.GetProductsAsync();
            var productVmList = new List<ProductVIewModel>();

            foreach (var p in products)
            {

                var productVm = new ProductVIewModel()
                {
                     Id = p.Id,
                     Name = p.Name,
                     Description = p.Description,
                     Price = p.Price,
                     Category = null,
                     Features = null,
                     Images = null,
                     Promotions = null,
                     Videos = null
                };

                productVmList.Add(productVm);
            }

            return productVmList;
        }

     }
}
