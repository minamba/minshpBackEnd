using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillingAddress = MinshpWebApp.Domain.Models.BillingAddress;

namespace MinshpWebApp.Dal.Repositories
{
    public class BillingAddressRepository : IBillingAddressRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public BillingAddressRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BillingAddress>> GetBillingAddressesAsync()
        {
            var BillingAddressEntities = await _context.BillingAddresses.Select(p => new BillingAddress
            {
                Id = p.Id,
                Address = p.Address,
                City = p.City,
                ComplementaryAddress = p.ComplementaryAddress,
                Country = p.Country,
                IdCustomer = p.IdCustomer,
                PostalCode = p.PostalCode,
            }).ToListAsync();

            return BillingAddressEntities;
        }


        public async Task<BillingAddress> UpdateBillingAddressAsync(BillingAddress model)
        {
            var BillingAddressToUpdate = await _context.BillingAddresses.FirstOrDefaultAsync(u => u.Id == model.Id && u.IdCustomer == model.IdCustomer);

            if (BillingAddressToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.ComplementaryAddress != null) BillingAddressToUpdate.ComplementaryAddress = model.ComplementaryAddress;
            if (model.Address != null) BillingAddressToUpdate.Address = model.Address;
            if (model.Country != null) BillingAddressToUpdate.Country = model.Country;
            if (model.PostalCode != null) BillingAddressToUpdate.PostalCode = model.PostalCode;
            if (model.City != null) BillingAddressToUpdate.City = model.City;


            await _context.SaveChangesAsync();

            return new BillingAddress()
            {
                Id = model.Id,
                ComplementaryAddress = model.ComplementaryAddress,
                Address = model.Address,
                Country = model.Country,
                PostalCode = model.PostalCode,
                City = model.City,
                IdCustomer = model.IdCustomer,
            };
        }


        public async Task<BillingAddress> AddBillingAddresssAsync(Domain.Models.BillingAddress model)
        {
            var newBillingAddress = new Dal.Entities.BillingAddress
            {
                Id = model.Id,
                ComplementaryAddress = model.ComplementaryAddress,
                Address = model.Address,
                Country = model.Country,
                PostalCode = model.PostalCode,
                City = model.City,
                IdCustomer = model.IdCustomer,
            };

            _context.BillingAddresses.Add(newBillingAddress);
            _context.SaveChanges();

            return new BillingAddress()
            {
                Id = model.Id,
                ComplementaryAddress = model.ComplementaryAddress,
                Address = model.Address,
                Country = model.Country,
                PostalCode = model.PostalCode,
                City = model.City,
                IdCustomer = model.IdCustomer,
            };
        }


        public async Task<bool> DeleteBillingAddresssAsync(int idBillingAddress)
        {
            var BillingAddressToDelete = await _context.BillingAddresses.FirstOrDefaultAsync(u => u.Id == idBillingAddress);

            if (BillingAddressToDelete == null)
                return false; // ou throw une exception;

            _context.BillingAddresses.Remove(BillingAddressToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
