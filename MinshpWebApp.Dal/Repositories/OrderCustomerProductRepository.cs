using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderCustomerProduct = MinshpWebApp.Domain.Models.OrderCustomerProduct;

namespace MinshpWebApp.Dal.Repositories
{
    public class OrderCustomerProductRepository : IOrderCustomerProductRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public OrderCustomerProductRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderCustomerProduct>> GetOrderCustomerProductsAsync()
        {
            var OrderCustomerProductEntities = await _context.OrderCustomerProducts.Select(p => new OrderCustomerProduct
            {
                Id = p.Id,
                CustomerId = p.CustomerId,
                OrderId = p.OrderId,
                ProductId = p.ProductId,
                Quantity = p.Quantity,
                ProductUnitPrice = p.ProductUnitPrice
            }).ToListAsync();

            return OrderCustomerProductEntities;
        }


        public async Task<OrderCustomerProduct> UpdateOrderCustomerProductsAsync(OrderCustomerProduct model)
        {
            var OrderCustomerProductToUpdate = await _context.OrderCustomerProducts.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (OrderCustomerProductToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.ProductId != null) OrderCustomerProductToUpdate.ProductId = model.ProductId;
            if (model.OrderId != null) OrderCustomerProductToUpdate.OrderId = model.OrderId;
            if (model.CustomerId != null) OrderCustomerProductToUpdate.CustomerId = model.CustomerId;
            if (model.Quantity != null) OrderCustomerProductToUpdate.Quantity = model.Quantity;
            if (model.ProductUnitPrice != null) OrderCustomerProductToUpdate.ProductUnitPrice = model.ProductUnitPrice;


            await _context.SaveChangesAsync();


            return new OrderCustomerProduct()
            {
                Id = model.Id,
                ProductId = model.ProductId,
                OrderId = model.OrderId,
                CustomerId = model.CustomerId,
                Quantity = model.Quantity,
                ProductUnitPrice = model.ProductUnitPrice
            };
        }


        public async Task<OrderCustomerProduct> AddOrderCustomerProductsAsync(Domain.Models.OrderCustomerProduct model)
        {
            var newOrderCustomerProduct = new Dal.Entities.OrderCustomerProduct
            {
                ProductId = model.ProductId,
                OrderId = model.OrderId,
                CustomerId = model.CustomerId,
                Quantity = model.Quantity,
                ProductUnitPrice = model.ProductUnitPrice
            };

            _context.OrderCustomerProducts.Add(newOrderCustomerProduct);
            _context.SaveChanges();

            return new OrderCustomerProduct()
            {
                Id = newOrderCustomerProduct.Id,
                ProductId = model.ProductId,
                OrderId = model.OrderId,
                CustomerId = model.CustomerId,
                Quantity = model.Quantity,
                ProductUnitPrice = model.ProductUnitPrice
            };
        }


        public async Task<bool> DeleteOrderCustomerProductsAsync(int idOrderCustomerProduct)
        {
            var OrderCustomerProductToDelete = await _context.OrderCustomerProducts.FirstOrDefaultAsync(u => u.Id == idOrderCustomerProduct);

            if (OrderCustomerProductToDelete == null)
                return false; // ou throw une exception;

            _context.OrderCustomerProducts.Remove(OrderCustomerProductToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
