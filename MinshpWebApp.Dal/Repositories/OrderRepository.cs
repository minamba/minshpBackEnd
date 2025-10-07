using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Dal.Utils;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using MinshpWebApp.Domain.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Order = MinshpWebApp.Domain.Models.Order;

namespace MinshpWebApp.Dal.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public OrderRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            var OrderEntities = await _context.Orders.Select(p => new Order
            {
                Id = p.Id,
                Date = p.Date,
                OrderNumber = p.OrderNumber,
                Status = p.Status,
                CustomerId = p.CustomerId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                DeliveryAmount = p.DeliveryAmount,
                TrackingNumber = p.TrackingNumber,
                BoxtalShipmentId = p.BoxtalShipmentId,
                DeliveryMode = p.DeliveryMode,
                LabelUrl = p.LabelUrl,
                Carrier = p.Carrier,
                RelayLabel = p.RelayLabel,
                RelayId = p.RelayId,
                ServiceCode = p.ServiceCode,
                TrackingLink = p.TrackingLink,
                CartDiscount = p.CartDiscount

            }).ToListAsync();

            return OrderEntities;
        }


        public async Task<Order> UpdateOrdersAsync(Order model)
        {
            var OrderToUpdate = await _context.Orders.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (OrderToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.Date != null) OrderToUpdate.Date = model.Date;
            if (model.Status != null) OrderToUpdate.Status = model.Status;
            if (model.CustomerId != null) OrderToUpdate.CustomerId = model.CustomerId;
            if (model.Amount != null) OrderToUpdate.Amount = model.Amount;
            if (model.PaymentMethod != null) OrderToUpdate.PaymentMethod = model.PaymentMethod;
            if (model.DeliveryAmount != null) OrderToUpdate.DeliveryAmount = model.DeliveryAmount;

            if (model.DeliveryMode != null) OrderToUpdate.DeliveryMode = model.DeliveryMode;
            if (model.Carrier != null) OrderToUpdate.Carrier = model.Carrier;
            if (model.ServiceCode != null) OrderToUpdate.ServiceCode = model.ServiceCode;
            if (model.RelayId != null) OrderToUpdate.RelayId = model.RelayId;
            if (model.RelayLabel != null) OrderToUpdate.RelayLabel = model.RelayLabel;
            if (model.BoxtalShipmentId != null) OrderToUpdate.BoxtalShipmentId = model.BoxtalShipmentId;
            if (model.TrackingNumber != null) OrderToUpdate.TrackingNumber = model.TrackingNumber;
            if (model.LabelUrl != null) OrderToUpdate.LabelUrl = model.LabelUrl;
            if (model.TrackingLink != null) OrderToUpdate.TrackingLink = model.TrackingLink;
            if (model.CartDiscount != null) OrderToUpdate.CartDiscount = model.CartDiscount;


            //if (OrderToUpdate.TrackingNumber == null)
            //    OrderToUpdate.TrackingNumber = model.TrackingNumber;

            await _context.SaveChangesAsync();


            return new Order()
            {
                Id = OrderToUpdate.Id,
                Date = OrderToUpdate.Date,
                Status = OrderToUpdate.Status,
                CustomerId = OrderToUpdate.CustomerId,
                Amount = OrderToUpdate.Amount,
                PaymentMethod = OrderToUpdate.PaymentMethod,
                DeliveryAmount = OrderToUpdate.DeliveryAmount,

                TrackingNumber = OrderToUpdate.TrackingNumber,
                BoxtalShipmentId = OrderToUpdate.BoxtalShipmentId,
                DeliveryMode = OrderToUpdate.DeliveryMode,
                LabelUrl = OrderToUpdate.LabelUrl,
                Carrier = OrderToUpdate.Carrier,
                RelayLabel = OrderToUpdate.RelayLabel,
                RelayId = OrderToUpdate.RelayId,
                ServiceCode = OrderToUpdate.ServiceCode,
                TrackingLink = OrderToUpdate.TrackingLink,
                CartDiscount = OrderToUpdate.CartDiscount
            };
        }


        public async Task<Order> AddOrdersAsync(Domain.Models.Order model)
        {
            var newOrder = new Dal.Entities.Order
            {
                Id = model.Id,
                Date = DateTime.Now,
                Status = model.Status,
                CustomerId = model.CustomerId,
                Amount = model.Amount,
                PaymentMethod = model.PaymentMethod,
                DeliveryAmount = model.DeliveryAmount

            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            return new Order()
            {
                Id = newOrder.Id,
                Date = newOrder.Date,
                OrderNumber = newOrder.OrderNumber,
                Status = newOrder.Status,
                CustomerId = newOrder.CustomerId,
                Amount = newOrder.Amount,
                PaymentMethod = newOrder.PaymentMethod,
                DeliveryAmount = newOrder.DeliveryAmount
            };
        }


        public async Task<bool> DeleteOrdersAsync(int idOrder)
        {
            var OrderToDelete = await _context.Orders.FirstOrDefaultAsync(u => u.Id == idOrder);

            if (OrderToDelete == null)
                return false; // ou throw une exception;

            _context.Orders.Remove(OrderToDelete);
            await _context.SaveChangesAsync();

            return true;
        }


        //BOXTAL 

        public async Task<Order> FindByShipmentIdAsync(string providerShipmentId)
        {
            var orderToFind = await _context.Orders.FirstOrDefaultAsync(o => o.BoxtalShipmentId == providerShipmentId);


            var order = new Order
            {
                Id = orderToFind.Id,
                Date = orderToFind.Date,
                Status = orderToFind.Status,
                CustomerId = orderToFind.CustomerId,
                Amount = orderToFind.Amount,
                PaymentMethod = orderToFind.PaymentMethod,
                DeliveryAmount = orderToFind.DeliveryAmount,
                
                DeliveryMode = orderToFind.DeliveryMode,
                Carrier = orderToFind.Carrier,
                ServiceCode = orderToFind.ServiceCode,
                RelayId = orderToFind.RelayId,
                RelayLabel = orderToFind.RelayLabel,
                BoxtalShipmentId = orderToFind.BoxtalShipmentId,
                TrackingNumber = orderToFind.TrackingNumber,
                LabelUrl = orderToFind.LabelUrl,
                TrackingLink = orderToFind.TrackingLink,
                CartDiscount = orderToFind.CartDiscount
            };

            return order;

        }


        public async Task<Order> GetByIdAsync(string id)
        {
            MinshpWebApp.Dal.Entities.Order orderTofind = new Entities.Order();

            orderTofind = await _context.Orders.FirstOrDefaultAsync(o => o.Id ==  int.Parse(id));
           
            if(orderTofind == null)
                orderTofind = await _context.Orders.FirstOrDefaultAsync(o => o.OrderNumber == id);

            if (orderTofind != null)
            {

                var order = new Order
                {
                    Id = orderTofind.Id,
                    Date = orderTofind.Date,
                    Status = orderTofind.Status,
                    CustomerId = orderTofind.CustomerId,
                    Amount = orderTofind.Amount,
                    PaymentMethod = orderTofind.PaymentMethod,
                    DeliveryAmount = orderTofind.DeliveryAmount,

                    DeliveryMode = orderTofind.DeliveryMode,
                    Carrier = orderTofind.Carrier,
                    ServiceCode = orderTofind.ServiceCode,
                    RelayId = orderTofind.RelayId,
                    RelayLabel = orderTofind.RelayLabel,
                    BoxtalShipmentId = orderTofind.BoxtalShipmentId,
                    TrackingNumber = orderTofind.TrackingNumber,
                    LabelUrl = orderTofind.LabelUrl,
                    TrackingLink = orderTofind.TrackingLink,
                    CartDiscount = orderTofind.CartDiscount
                };


                return order;
            }

            return null;
        }



        public async Task<IEnumerable<Order>> GetOrdersByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids.Distinct().ToList();
            var orderEntities = await _context.Orders
                .AsNoTracking()
                .Where(p => idList.Contains(p.Id))
                .OrderByDescending(p => p.Date)
                .Select(p => new Order
                {
                    Id = p.Id,
                    Date = p.Date,
                    OrderNumber = p.OrderNumber,
                    Status = p.Status,
                    CustomerId = p.CustomerId,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    DeliveryAmount = p.DeliveryAmount,
                    TrackingNumber = p.TrackingNumber,
                    BoxtalShipmentId = p.BoxtalShipmentId,
                    DeliveryMode = p.DeliveryMode,
                    LabelUrl = p.LabelUrl,
                    Carrier = p.Carrier,
                    RelayLabel = p.RelayLabel,
                    RelayId = p.RelayId,
                    ServiceCode = p.ServiceCode,
                    TrackingLink = p.TrackingLink,
                    CartDiscount = p.CartDiscount
                })
                .ToListAsync();

            // Remet l'ordre des IDs paginés (important pour garder le tri)
            var order = idList.Select((id, i) => new { id, i }).ToDictionary(x => x.id, x => x.i);
            return orderEntities.OrderBy(p => order[p.Id]).ToList();
        }



        public async Task<PageResult<int>> PageOrderIdsAsync(PageRequest req, CancellationToken ct = default)
        {
            var q = _context.Orders.AsNoTracking().OrderByDescending( o => o.Date);

            // champs de recherche génériques
            var search = new Expression<Func<Dal.Entities.Order, string?>>[]
            {
                p => p.OrderNumber, p => p.Status, p => p.Customer.ClientNumber1, p => p.Customer.LastName, p => p.Customer.FirstName, p => p.Customer.Email
            };



            // filtres génériques (clé = Filter.<Key> côté front)
            var filters = new Dictionary<string, Func<IQueryable<Dal.Entities.Order>, string, IQueryable<Dal.Entities.Order>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["OrderNumber"] = (qq, v) => string.IsNullOrWhiteSpace(v) ? qq : qq.Where(p => p.OrderNumber != null && p.OrderNumber.Equals(v, StringComparison.OrdinalIgnoreCase)),
                ["Status"] = (qq, v) => string.IsNullOrWhiteSpace(v) ? qq : qq.Where(p => p.Status != null && p.Status.Equals(v, StringComparison.OrdinalIgnoreCase)),
            };

            // On page d'abord sur les IDs (rapide + stable), tri par défaut
            var page = await PagedQuery.ExecuteAsync<Dal.Entities.Order, int>(
                query: q,
                req: req,
                searchFields: search,
                filterHandlers: filters,
                defaultSort: "Date:desc",
                selector: p => p.Id,
                ct: ct
            );

            return page;
        }

    }
}
