using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryAddress = MinshpWebApp.Domain.Models.DeliveryAddress;

namespace MinshpWebApp.Dal.Repositories
{
    public class DeliveryAddressRepository : IDeliveryAddressRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public DeliveryAddressRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DeliveryAddress>> GetDeliveryAddressesAsync()
        {
            var DeliveryAddressEntities = await _context.DeliveryAddresses.Select(p => new DeliveryAddress
            {
                Id = p.Id,
                Address = p.Address,
                City = p.City,
                ComplementaryAddress = p.ComplementaryAddress,
                Country = p.Country,
                IdCustomer = p.IdCustomer,
                PostalCode = p.PostalCode,
                Favorite = p.Favorite,
                LastName = p.LastName,
                FirstName = p.FirstName,
                Phone = p.Phone
            }).ToListAsync();

            return DeliveryAddressEntities;
        }


        public async Task<DeliveryAddress> UpdateDeliveryAddressAsync(DeliveryAddress model)
        {
            var DeliveryAddressToUpdate = await _context.DeliveryAddresses.FirstOrDefaultAsync(u => u.Id == model.Id && u.IdCustomer == model.IdCustomer);

            if (DeliveryAddressToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.ComplementaryAddress != null) DeliveryAddressToUpdate.ComplementaryAddress = model.ComplementaryAddress;
            if (model.Address != null) DeliveryAddressToUpdate.Address = model.Address;
            if (model.Country != null) DeliveryAddressToUpdate.Country = model.Country;
            if (model.PostalCode != null) DeliveryAddressToUpdate.PostalCode = model.PostalCode;
            if (model.Favorite != null) DeliveryAddressToUpdate.Favorite = model.Favorite;
            if (model.City != null) DeliveryAddressToUpdate.City = model.City;
            if (model.LastName != null) DeliveryAddressToUpdate.LastName = model.LastName;
            if (model.FirstName != null) DeliveryAddressToUpdate.FirstName = model.FirstName;
            if (model.Phone != null) DeliveryAddressToUpdate.Phone = model.Phone;



            await _context.SaveChangesAsync();

            return new DeliveryAddress()
            {
                Id = model.Id,
                ComplementaryAddress = model.ComplementaryAddress,
                Address = model.Address,
                Country = model.Country,
                PostalCode = model.PostalCode,
                City = model.City,
                IdCustomer = model.IdCustomer,
                Favorite = model.Favorite,
                LastName = model.LastName,
                FirstName = model.FirstName,
                Phone = model.Phone
            };
        }


        public async Task<DeliveryAddress> AddDeliveryAddresssAsync(Domain.Models.DeliveryAddress model)
        {
            var newDeliveryAddress = new Dal.Entities.DeliveryAddress
            {
                Id = model.Id,
                ComplementaryAddress = model.ComplementaryAddress,
                Address = model.Address,
                Country = model.Country,
                PostalCode = model.PostalCode,
                City = model.City,
                IdCustomer = model.IdCustomer,
                Favorite = model.Favorite,
                LastName = model.LastName,
                FirstName = model.FirstName,
                Phone = model.Phone
            };

            _context.DeliveryAddresses.Add(newDeliveryAddress);
            _context.SaveChanges();

            return new DeliveryAddress()
            {
                Id = model.Id,
                ComplementaryAddress = model.ComplementaryAddress,
                Address = model.Address,
                Country = model.Country,
                PostalCode = model.PostalCode,
                City = model.City,
                IdCustomer = model.IdCustomer,
                Favorite = model.Favorite,
                LastName = model.LastName,
                FirstName = model.FirstName,
                Phone = model.Phone
            };
        }


        public async Task<bool> DeleteDeliveryAddresssAsync(int idDeliveryAddress)
        {
            var DeliveryAddressToDelete = await _context.DeliveryAddresses.FirstOrDefaultAsync(u => u.Id == idDeliveryAddress);

            if (DeliveryAddressToDelete == null)
                return false; // ou throw une exception;

            _context.DeliveryAddresses.Remove(DeliveryAddressToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
