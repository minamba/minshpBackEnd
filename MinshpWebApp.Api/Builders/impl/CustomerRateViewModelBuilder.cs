using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;

namespace MinshpWebApp.Api.Builders.impl
{
    public class CustomerRateViewModelBuilder : ICustomerRateViewModelBuilder
    {

        private IMapper _mapper;
        private ICustomerRateService _CustomerRateService;
        private ICustomerViewModelBuilder _CustomerViewModelBuilder;
        private IProductViewModelBuilder _ProductViewModelBuilder;
        private ITelegramViewModelBuilder _telegramViewModelBuilder;
        private IOrderViewModelBuilder _orderViewModelBuilder;
        private IOrderCustomerProductViewModelBuilder _orderCustomerProductViewModelBuilder;



        public CustomerRateViewModelBuilder(ICustomerRateService CustomerRateService, ICustomerViewModelBuilder CustomerViewModelBuilder, IProductViewModelBuilder ProductViewModelBuilder, IOrderViewModelBuilder orderViewModelBuilder, IOrderCustomerProductViewModelBuilder orderCustomerProductViewModelBuilder, ITelegramViewModelBuilder telegramViewModelBuilder, IMapper mapper)
        {
            _mapper = mapper;
            _CustomerRateService = CustomerRateService;
            _CustomerViewModelBuilder = CustomerViewModelBuilder;
            _ProductViewModelBuilder = ProductViewModelBuilder;
            _orderViewModelBuilder = orderViewModelBuilder;
            _orderCustomerProductViewModelBuilder = orderCustomerProductViewModelBuilder;
            _telegramViewModelBuilder = telegramViewModelBuilder;
        }

        public async Task<CustomerRate> AddCustomerRateAsync(CustomerRateRequest model)
        {
            var newCustomerRate = _mapper.Map<CustomerRate>(model);
            var customer = (await _CustomerViewModelBuilder.GetCustomersAsync()).FirstOrDefault(c => c.Id == model.IdCustomer);
            var product = (await _ProductViewModelBuilder.GetProductsAsync()).FirstOrDefault(p => p.Id == model.IdProduct);


            var result = await _CustomerRateService.AddCustomerRateAsync(newCustomerRate);


            if(result != null)
            {
                var telgramRequest = new TelegramRequest()
                {
                    Mail = customer.Email,
                    Brand = product.Brand,
                    Model = product.Model,
                    Review = model.Message,
                    Date = DateTime.Now.ToString("dd/MM/yyyy"),
                };

                await _telegramViewModelBuilder.SendReviewMessage(telgramRequest);
            }

            return result;
        }


        public async Task<bool> DeleteCustomerRateAsync(int idCustomerRate)
        {
            return await _CustomerRateService.DeleteCustomerRateAsync(idCustomerRate);
        }


        public async Task<IEnumerable<CustomerRateViewModel>> GetCustomerRatesAsync()
        {
            var CustomerRates = await _CustomerRateService.GetCustomerRatesAsync();
            var customers = await _CustomerViewModelBuilder.GetCustomersAsync();
            var products =  await _ProductViewModelBuilder.GetProductsAsync();
            //var orders = await _orderViewModelBuilder.GetOrdersAsync();
            //var ordersCustomerProduct = await _orderCustomerProductViewModelBuilder.GetOrderCustomerProductsAsync();


            var result = _mapper.Map<IEnumerable<CustomerRateViewModel>>(CustomerRates);

            foreach(var r in result)
            {
                    r.customer = customers.FirstOrDefault(c => c.Id == r.IdCustomer);
                    r.product = products.FirstOrDefault(p => p.Id == r.IdProduct);
            }

            result.OrderByDescending(x => x.CreationDate).ToList();

            return result;
        }

        public async Task<CustomerRate> UpdateCustomerRateAsync(CustomerRateRequest model)
        {
            var CustomerRate = _mapper.Map<CustomerRate>(model);

            return await _CustomerRateService.UpdateCustomerRateAsync(CustomerRate);
        }
    }
}
