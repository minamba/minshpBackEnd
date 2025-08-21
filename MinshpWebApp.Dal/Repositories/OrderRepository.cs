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
                Quantity = p.Quantity,
                Status = p.Status,
                IdCustomer = p.IdCustomer,
                IdProduct = p.Id_product,
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
            if (model.OrderNumber != null) OrderToUpdate.OrderNumber = model.OrderNumber;
            if (model.Quantity != null) OrderToUpdate.Quantity = model.Quantity;
            if (model.Status != null) OrderToUpdate.Status = model.Status;
            if (model.IdCustomer != null) OrderToUpdate.IdCustomer = model.IdCustomer;
            if (model.IdProduct != null) OrderToUpdate.Id_product = model.IdProduct;

            await _context.SaveChangesAsync();


            return new Order()
            {
                Id = model.Id,
                Date = model.Date,
                OrderNumber = model.OrderNumber,
                Quantity = model.Quantity,
                Status = model.Status,
                IdCustomer = model.IdCustomer,
                IdProduct = model.IdProduct,
            };
        }


        public async Task<Order> AddOrdersAsync(Domain.Models.Order model)
        {
            var newOrder = new Dal.Entities.Order
            {
                Id = model.Id,
                Date = model.Date,
                OrderNumber = model.OrderNumber,
                Quantity = model.Quantity,
                Status = model.Status,
                IdCustomer = model.IdCustomer,
                Id_product = model.IdProduct,
            };

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            return new Order()
            {
                Id = model.Id,
                Date = model.Date,
                OrderNumber = model.OrderNumber,
                Quantity = model.Quantity,
                Status = model.Status,
                IdCustomer = model.IdCustomer,
                IdProduct = model.IdProduct,
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
    }
}
