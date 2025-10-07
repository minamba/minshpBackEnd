using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.Utils;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using MinshpWebApp.Domain.Services.impl;
using MinshpWebApp.Domain.Services.Shipping;
using Stripe;
using System.Linq;

namespace MinshpWebApp.Api.Builders.impl
{
    public class OrderViewModelBuilder : IOrderViewModelBuilder
    {
        private IMapper _mapper;
        private IOrderService _orderService;
        private IOrderCustomerProductViewModelBuilder _orderCustomerProductViewModelBuilder;
        private ICustomerViewModelBuilder _customerViewModelBuilder;
        private IProductViewModelBuilder _productViewModelBuilder;
        private IShippingProvider _shippingProvider;


        public OrderViewModelBuilder(IOrderService orderService,  IMapper mapper, IOrderCustomerProductViewModelBuilder orderCustomerProductVm, ICustomerViewModelBuilder customerVm, IProductViewModelBuilder productVm, IShippingProvider shippingProvider)
        {
            _mapper = mapper;
            _orderService = orderService;
            _productViewModelBuilder = productVm;
            _customerViewModelBuilder = customerVm;
            _orderCustomerProductViewModelBuilder = orderCustomerProductVm;
            _shippingProvider = shippingProvider;
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


                //mis a jour des differentes informations de tracking et du status, dans le cas ou ça n'a pas pu etre recupéré lors de la creation de commande
                if (Order != null && Order.BoxtalShipmentId != null)
                {
                    var tracking = await _shippingProvider.GetShippingTrackingAsync(Order.BoxtalShipmentId);

                    if (tracking != null)
                    {
                        var trackingStatus = TrackingStatus.GetTrackingStatus(tracking.Status);

                         Order.TrackingNumber = tracking.TrackingNumber;
                         Order.TrackingLink = tracking.PackageTrackingUrl;
                         Order.Status = trackingStatus;
                         _orderService.UpdateOrdersAsync(Order);
                    }
                }


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
                        TrackingLink = Order.TrackingLink,
                        CartDiscount = Order.CartDiscount
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


        public async Task<IEnumerable<Order>> GetOrdersByIdsAsync(IEnumerable<int> ids)
        {
            return await _orderService.GetOrdersByIdsAsync(ids);
        }


        public async Task<PageResult<OrderViewModel>> PageOrderIdsAsync(PageRequest req, CancellationToken ct = default)
        {
            // 1) Page d’IDs (service)
            var idPage = await _orderService.PageOrderIdsAsync(req, ct);
            if (idPage == null || idPage.Items.Count == 0)
            {
                return new PageResult<OrderViewModel>
                {
                    Items = Array.Empty<OrderViewModel>(),
                    TotalCount = idPage?.TotalCount ?? 0,
                    Page = idPage?.Page ?? req.Page,
                    PageSize = idPage?.PageSize ?? req.PageSize
                };
            }

            var orderIds = idPage.Items.ToHashSet();

            // 2) Charger uniquement les commandes de la page (séquentiel)
            var orders = (await _orderService.GetOrdersByIdsAsync(idPage.Items)).ToList();

            // 3) Charger le reste (séquentiel pour éviter la concurrence sur le même DbContext)
            var customers = (await _customerViewModelBuilder.GetCustomersAsync()).ToList();
            var products = (await _productViewModelBuilder.GetProductsAsync()).ToList();
            var ocpsAll = (await _orderCustomerProductViewModelBuilder.GetOrderCustomerProductsAsync())
                            .Where(x => orderIds.Contains((int)x.OrderId))
                            .ToList();

            // 4) Dicos utiles
            var productsById = products.ToDictionary(p => p.Id, p => p);
            var ocpsByOrder = ocpsAll
                .GroupBy(ocp => (int)ocp.OrderId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 5) Reconstruction des VMs (comme ta version non paginée)
            var vmList = new List<OrderViewModel>();
            var orderIndex = idPage.Items
                .Select((id, i) => new { id, i })
                .ToDictionary(x => x.id, x => x.i);

            foreach (var order in orders)
            {
                var ocps = ocpsByOrder.TryGetValue(order.Id, out var lines)
                    ? lines
                    : new List<OrderCustomerProductViewModel>(); // adapte le type si différent

                var productVmList = new List<ProductVIewModel>();

                foreach (var ocp in ocps)
                {
                    if (!productsById.TryGetValue((int)ocp.ProductId, out var p)) continue;

                    // clone léger pour ne pas muter d’autres références
                    var prod = new ProductVIewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Brand = p.Brand,
                        Model = p.Model,
                        Description = p.Description,
                        Price = p.Price,
                        PriceTtc = p.PriceTtc,
                        PriceHtPromoted = p.PriceHtPromoted,
                        PriceHtCategoryCodePromoted = p.PriceHtCategoryCodePromoted,
                        Tva = p.Tva,
                        TaxWithoutTvaAmount = p.TaxWithoutTvaAmount,
                        Images = p.Images,
                        Promotions = p.Promotions,
                    };

                    int quantity = ocp.Quantity ?? 1;

                    if (prod.PriceHtCategoryCodePromoted != null)
                        prod.PriceHtCategoryCodePromoted *= quantity;

                    if (prod.PriceHtPromoted != null && prod.PriceHtCategoryCodePromoted == null)
                        prod.PriceHtPromoted *= quantity;

                    if (prod.PriceTtc != null && prod.PriceHtPromoted == null && prod.PriceHtCategoryCodePromoted == null)
                        prod.PriceTtc *= quantity;

                    productVmList.Add(prod);
                }

                var customer = customers.FirstOrDefault(c => c.Id == order.CustomerId);

                // tracking identique à ta version non-paginée

                if (order != null && order.BoxtalShipmentId != null)
                {
                    var tracking = await _shippingProvider.GetShippingTrackingAsync(order.BoxtalShipmentId);
                    if (tracking != null)
                    {
                        var trackingStatus = TrackingStatus.GetTrackingStatus(tracking.Status);
   
                        order.TrackingNumber = tracking.TrackingNumber;
                        order.TrackingLink = tracking.PackageTrackingUrl;
                        order.Status = trackingStatus;
                        await _orderService.UpdateOrdersAsync(order); // ct si dispo
                    }
                }
                

                var vm = new OrderViewModel
                {
                    Id = order.Id,
                    Amount = order.Amount,
                    DeliveryAmount = order.DeliveryAmount,
                    Date = order.Date,
                    PaymentMethod = order.PaymentMethod,
                    Status = order.Status,
                    OrderNumber = order.OrderNumber,
                    Customer = customer,
                    Products = productVmList,
                    TrackingNumber = order.TrackingNumber,
                    DeliveryMode = order.DeliveryMode,
                    BoxtalShipmentId = order.BoxtalShipmentId,
                    LabelUrl = order.LabelUrl,
                    Carrier = order.Carrier,
                    RelayId = order.RelayId,
                    RelayLabel = order.RelayLabel,
                    ServiceCode = order.ServiceCode,
                    TrackingLink = order.TrackingLink,
                    CartDiscount = order.CartDiscount
                };

                vmList.Add(vm);
            }

            // 6) Respect de l’ordre initial de pagination
            vmList = vmList.OrderBy(vm => orderIndex[vm.Id]).ToList();

            // 7) Résultat
            return new PageResult<OrderViewModel>
            {
                Items = vmList,
                TotalCount = idPage.TotalCount,
                Page = idPage.Page,
                PageSize = idPage.PageSize
            };
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
