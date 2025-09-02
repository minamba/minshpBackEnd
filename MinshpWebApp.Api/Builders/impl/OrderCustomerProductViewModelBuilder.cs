using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Services;
using OrderCustomerProduct = MinshpWebApp.Domain.Models.OrderCustomerProduct;

namespace MinshpWebApp.Api.Builders.impl
{
    public class OrderCustomerProductViewModelBuilder : IOrderCustomerProductViewModelBuilder
    {
        private IMapper _mapper;
        private IOrderCustomerProductService _OrderCustomerProductService;


        public OrderCustomerProductViewModelBuilder(IOrderCustomerProductService OrderCustomerProductService, IMapper mapper)
        {
            _mapper = mapper;
            _OrderCustomerProductService = OrderCustomerProductService;
        }

        public async Task<OrderCustomerProduct> AddOrderCustomerProductsAsync(OrderCustomerProductRequest model)
        {
            if (model.OrderId != null && model.ProductId != null && model.CustomerId != null)
            {
                return await _OrderCustomerProductService.AddOrderCustomerProductsAsync(_mapper.Map<OrderCustomerProduct>(model));
            }
            return null;
        }

        public async Task<bool> DeleteOrderCustomerProductsAsync(OrderCustomerProductRequest model)
        {
            var orderCustomerProduct = (await _OrderCustomerProductService.GetOrderCustomerProductsAsync()).FirstOrDefault( o => o.ProductId == model.ProductId && o.OrderId == model.OrderId && o.CustomerId == model.CustomerId);

            return await _OrderCustomerProductService.DeleteOrderCustomerProductsAsync(orderCustomerProduct.Id);
        }

        public async Task<IEnumerable<OrderCustomerProductViewModel>> GetOrderCustomerProductsAsync()
        {
            var result = await _OrderCustomerProductService.GetOrderCustomerProductsAsync();

            return _mapper.Map<IEnumerable<OrderCustomerProductViewModel>>(result);
        }

        public async Task<OrderCustomerProduct> UpdateOrderCustomerProductsAsync(OrderCustomerProductRequest model)
        {
            var getOrderCustomerProductId = (await _OrderCustomerProductService.GetOrderCustomerProductsAsync()).FirstOrDefault(o => o.ProductId == model.ProductId && o.OrderId == model.OrderId);

            var OrderCustomerProduct = _mapper.Map<OrderCustomerProduct>(model);
            OrderCustomerProduct.Id = getOrderCustomerProductId.Id;
            var result = await _OrderCustomerProductService.UpdateOrderCustomerProductsAsync(OrderCustomerProduct);

            return result;
        }
    }
}
