using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.Utils;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using MinshpWebApp.Domain.Services.impl;

namespace MinshpWebApp.Api.Builders.impl
{
    public class OrderViewModelBuilder : IOrderViewModelBuilder
    {
        private IMapper _mapper;
        private IOrderService _orderService;
        private IOrderCustomerProductViewModelBuilder _orderCustomerProductViewModelBuilder;
        private ICustomerViewModelBuilder _customerViewModelBuilder;
        private IProductViewModelBuilder _productViewModelBuilder;


        public OrderViewModelBuilder(IOrderService orderService,  IMapper mapper, IOrderCustomerProductViewModelBuilder orderCustomerProductVm, ICustomerViewModelBuilder customerVm, IProductViewModelBuilder productVm)
        {
            _mapper = mapper;
            _orderService = orderService;
            _productViewModelBuilder = productVm;
            _customerViewModelBuilder = customerVm;
            _orderCustomerProductViewModelBuilder = orderCustomerProductVm;
        }

        public async Task<Order> AddOrdersAsync(OrderRequest model)
        {
            return await _orderService.AddOrdersAsync(_mapper.Map<Order>(model));
        }

        public async Task<bool> DeleteOrdersAsync(int idOrder)
        {
            return await _orderService.DeleteOrdersAsync(idOrder);
        }

        public async Task<Order> FindByShipmentIdAsync(string providerShipmentId)
        {
            return await _orderService.FindByShipmentIdAsync(providerShipmentId);
        }

        public async Task<Order> GetByIdAsync(string id)
        {
            return await (_orderService.GetByIdAsync(id));
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrdersAsync()
        {
            var orders = await _orderService.GetOrdersAsync();
            var customers = await _customerViewModelBuilder.GetCustomersAsync();
            var products = await _productViewModelBuilder.GetProductsAsync();
            var orderCustomerProducts = await _orderCustomerProductViewModelBuilder.GetOrderCustomerProductsAsync();
            string trackingLink = null;

            var groupOrders =
                from o in orders
                    // LEFT JOIN vers les lignes de commande
                join ocp in orderCustomerProducts on o.Id equals ocp.OrderId into ocps
                select new
                {
                    OrderId = o.Id,
                    // Pour ces lignes, on joint (inner) aux produits : s'il n'y a pas de ligne, la liste sera vide
                    Products = (
                        from x in ocps
                        join p in products on x.ProductId equals p.Id
                        select p
                    ).ToList()
                };



            var orderVmList = new List<OrderViewModel>();
            var ProductVmList = new List<ProductVIewModel>();
            CustomerViewModel customer = null;
            ProductVIewModel product = null;

            foreach (var o in groupOrders)
            {
                var Order = orders.FirstOrDefault(or => or.Id == o.OrderId);

                foreach (var p in o.Products)
                {
                    int? quantity = orderCustomerProducts.FirstOrDefault(ocp => ocp.OrderId == Order.Id && ocp.ProductId == p.Id).Quantity;

                        if (p.PriceHtCategoryCodePromoted != null)
                            p.PriceHtCategoryCodePromoted = p.PriceHtCategoryCodePromoted * quantity;

                        if (p.PriceHtPromoted != null && p.PriceHtCategoryCodePromoted == null)
                            p.PriceHtPromoted = p.PriceHtPromoted * quantity;

                        if (p.PriceTtc != null && p.PriceHtPromoted == null && p.PriceHtCategoryCodePromoted == null)
                            p.PriceTtc = p.PriceTtc * quantity;

                            ProductVmList.Add(p);
                }

                customer = customers.FirstOrDefault(c => c.Id == Order.CustomerId);


                if (customer.DeliveryAddresses.Count() > 0)
                {
                    var customerDeliveryAdress = customer.DeliveryAddresses.FirstOrDefault(d => d.Favorite == true);
                    trackingLink = TrackingLink.Build(Order.Carrier, Order.TrackingNumber, customerDeliveryAdress.PostalCode.ToString());
                }

       
                
                Console.WriteLine("fock");
                var ordervm = new OrderViewModel
                    {
                        Id = Order.Id,
                        Amount = Order.Amount,
                        DeliveryAmount = Order.DeliveryAmount,
                        Date = Order.Date,
                        PaymentMethod = Order.PaymentMethod,
                        Status = Order.Status,
                        OrderNumber = Order.OrderNumber,
                        Customer = customer,
                        Products = ProductVmList,
                        TrackingNumber = Order.TrackingNumber,
                        DeliveryMode = Order.DeliveryMode,
                        BoxtalShipmentId = Order.BoxtalShipmentId,
                        LabelUrl = Order.LabelUrl,
                        Carrier = Order.Carrier,
                        RelayId = Order.RelayId,
                        RelayLabel = Order.RelayLabel,
                        ServiceCode = Order.ServiceCode,
                        TrackingLink = trackingLink
                };
                    orderVmList.Add(ordervm);
                    ProductVmList = new List<ProductVIewModel>();
            }

            return orderVmList;
        }

        public async Task<Order> UpdateOrdersAsync(OrderRequest model)
        {
            var order = _mapper.Map<Order>(model);
            var result = await _orderService.UpdateOrdersAsync(order);

            return result;
        }


        private async Task<decimal?> CalculTotalAmount(IEnumerable<ProductVIewModel> lsproduct)
        {
            decimal? totalamount = 0;

            foreach (var p in lsproduct)
            {
                var amount = 0;

                if (p.PriceHtCategoryCodePromoted != null)
                    totalamount += p.PriceHtCategoryCodePromoted;

                if(p.PriceHtPromoted != null && p.PriceHtCategoryCodePromoted == null)
                    totalamount += p.PriceHtPromoted;

                if (p.PriceTtc != null && p.PriceHtPromoted == null && p.PriceHtCategoryCodePromoted == null)
                    totalamount += p.PriceTtc;
            }

            return totalamount;
        }
    }
}
