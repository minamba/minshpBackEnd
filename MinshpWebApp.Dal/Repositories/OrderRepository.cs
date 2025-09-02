using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                DeliveryAmount = p.DeliveryAmount
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

            await _context.SaveChangesAsync();


            return new Order()
            {
                Id = model.Id,
                Date = model.Date,
                Status = model.Status,
                CustomerId = model.CustomerId,
                Amount = model.Amount,
                PaymentMethod = model.PaymentMethod,
                DeliveryAmount = model.DeliveryAmount
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
                Date = model.Date,
                OrderNumber = newOrder.OrderNumber,
                Status = model.Status,
                CustomerId = model.CustomerId,
                Amount = model.Amount,
                PaymentMethod = model.PaymentMethod,
                DeliveryAmount = model.DeliveryAmount
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
            };

            return order;

        }


        public async Task<Order> GetByIdAsync(int id)
        {
            var orderToFind = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);


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
            };

            return order;
        }

    }
}
