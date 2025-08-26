using AutoMapper;
using Microsoft.AspNetCore.Identity;
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

        public CustomerRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            var CustomerEntities = await _context.Customers.Select(p => new Customer
            {
                Id = p.Id,
                LastName = p.LastName,
                FirstName = p.FirstName,
                PhoneNumber = p.PhoneNumber,
                Actif = p.Actif,
                BirthDate = p.BirthDate,
                Civilite = p.Civilite,
                ClientNumber = p.ClientNumber1,
                Email = p.Email,
                IdAspNetUser = p.IdAspNetUser,
                Pseudo = p.Pseudo,
                
               
            }).ToListAsync();

            return CustomerEntities;
        }


        public async Task<Customer> UpdateCustomersAsync(Customer model)
        {
            var CustomerToUpdate = await _context.Customers.FirstOrDefaultAsync(u => u.IdAspNetUser == model.IdAspNetUser);

            if (CustomerToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.LastName != null) CustomerToUpdate.LastName = model.LastName;
            if (model.FirstName != null) CustomerToUpdate.FirstName = model.FirstName;
            if (model.PhoneNumber != null) CustomerToUpdate.PhoneNumber = model.PhoneNumber;
            if (model.Actif != null) CustomerToUpdate.Actif = model.Actif;
            if (model.BirthDate != null) CustomerToUpdate.BirthDate = model.BirthDate;
            if (model.Civilite != null) CustomerToUpdate.Civilite = model.Civilite;
            if (model.Email != null) CustomerToUpdate.Email = model.Email;
            if (model.Pseudo != null) CustomerToUpdate.Pseudo = model.Pseudo;



            await _context.SaveChangesAsync();


            return new Customer()
            {
                Id = model.Id,
                LastName = model.LastName,
                FirstName = model.FirstName,
                PhoneNumber = model.PhoneNumber,
                Actif = model.Actif,
                BirthDate = model.BirthDate,
                Civilite = model.Civilite,
                ClientNumber = model.ClientNumber1,
                Email = model.Email,
                IdAspNetUser = model.IdAspNetUser,
                Pseudo = model.Pseudo
            };
        }


        public async Task<Customer> AddCustomersAsync(Domain.Models.Customer model)
        {
            var newCustomer = new Dal.Entities.Customer
            {
                Id = model.Id,
                LastName = model.LastName,
                FirstName = model.FirstName,
                PhoneNumber = model.PhoneNumber,
                Actif = model.Actif,
                BirthDate = model.BirthDate,
                Civilite = model.Civilite,
                Email = model.Email,
                IdAspNetUser = model.IdAspNetUser,
                Pseudo = model.Pseudo
            };

            _context.Customers.Add(newCustomer);
            _context.SaveChanges();

            return new Customer()
            {
                Id = newCustomer.Id,
                LastName = model.LastName,
                FirstName = model.FirstName,
                PhoneNumber = model.PhoneNumber,
                Actif = model.Actif,
                BirthDate = model.BirthDate,
                Civilite = model.Civilite,
                ClientNumber = newCustomer.ClientNumber1,
                Email = model.Email,
                IdAspNetUser = model.IdAspNetUser,
                Pseudo = model.Pseudo
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
