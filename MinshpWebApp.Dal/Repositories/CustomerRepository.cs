using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Customer = MinshpWebApp.Domain.Models.Customer;

namespace MinshpWebApp.Dal.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public CustomerRepository()
        {
            _context = new MinshpDatabaseContext();
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            var CustomerEntities = await _context.Customers.Select(p => new Customer
            {
                Id = p.Id,
                LastName = p.LastName,
                FirstName = p.FirstName,
                BillingAddress = p.BillingAddress,
                DeliveryAddress = p.DeliveryAddress,
                Password = p.Password,
                PhoneNumber = p.PhoneNumber,
            }).ToListAsync();

            return CustomerEntities;
        }


        public async Task<Customer> UpdateCustomersAsync(Customer model)
        {
            var CustomerToUpdate = await _context.Customers.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (CustomerToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.LastName != null) CustomerToUpdate.LastName = model.LastName;
            if (model.FirstName != null) CustomerToUpdate.FirstName = model.FirstName;
            if (model.BillingAddress != null) CustomerToUpdate.BillingAddress = model.BillingAddress;
            if (model.DeliveryAddress != null) CustomerToUpdate.DeliveryAddress = model.DeliveryAddress;
            if (model.Password != null) CustomerToUpdate.Password = model.Password;
            if (model.PhoneNumber != null) CustomerToUpdate.PhoneNumber = model.PhoneNumber;

            await _context.SaveChangesAsync();


            return new Customer()
            {
                Id = model.Id,
                LastName = model.LastName,
                FirstName = model.FirstName,
                BillingAddress = model.BillingAddress,
                DeliveryAddress = model.DeliveryAddress,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
            };
        }


        public async Task<Customer> AddCustomersAsync(Domain.Models.Customer model)
        {
            var newCustomer = new Dal.Entities.Customer
            {
                Id = model.Id,
                LastName = model.LastName,
                FirstName = model.FirstName,
                BillingAddress = model.BillingAddress,
                DeliveryAddress = model.DeliveryAddress,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
            };

            _context.Customers.Add(newCustomer);
            _context.SaveChanges();

            return new Customer()
            {
                Id = model.Id,
                LastName = model.LastName,
                FirstName = model.FirstName,
                BillingAddress = model.BillingAddress,
                DeliveryAddress = model.DeliveryAddress,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
            };
        }


        public async Task<bool> DeleteCustomersAsync(int idCustomer)
        {
            var CustomerToDelete = await _context.Customers.FirstOrDefaultAsync(u => u.Id == idCustomer);

            if (CustomerToDelete == null)
                return false; // ou throw une exception;

            _context.Customers.Remove(CustomerToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
