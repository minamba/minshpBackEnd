using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;

namespace MinshpWebApp.Api.Builders.impl
{
    public class OrderViewModelBuilder : IOrderViewModelBuilder
    {
        private IMapper _mapper;
        private IOrderService _orderService;


        public OrderViewModelBuilder(IOrderService orderService, IMapper mapper)
        {
            _mapper = mapper;
            _orderService = orderService;
        }

        public async Task<Order> AddOrdersAsync(OrderRequest model)
        {
            return await _orderService.AddOrdersAsync(_mapper.Map<Order>(model));
        }

        public async Task<bool> DeleteOrdersAsync(int idOrder)
        {
            return await _orderService.DeleteOrdersAsync(idOrder);
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrdersAsync()
        {
            var result = await _orderService.GetOrdersAsync();

            return _mapper.Map<IEnumerable<OrderViewModel>>(result);
        }

        public async Task<Order> UpdateOrdersAsync(OrderRequest model)
        {
            var order = _mapper.Map<Order>(model);
            var result = await _orderService.UpdateOrdersAsync(order);

            return result;
        }
    }
}
