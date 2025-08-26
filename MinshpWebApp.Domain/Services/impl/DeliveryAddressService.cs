using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class DeliveryAddressService : IDeliveryAddressService
    {
        IDeliveryAddressRepository _repository;


        public DeliveryAddressService(IDeliveryAddressRepository repository)
        {
            _repository = repository;
        }
        public async Task<DeliveryAddress> AddDeliveryAddresssAsync(DeliveryAddress model)
        {
            return await _repository.AddDeliveryAddresssAsync(model);
        }

        public async Task<bool> DeleteDeliveryAddresssAsync(int idDeliveryAddress)
        {
            return await _repository.DeleteDeliveryAddresssAsync(idDeliveryAddress);
        }

        public async Task<IEnumerable<DeliveryAddress>> GetDeliveryAddressesAsync()
        {
            return await _repository.GetDeliveryAddressesAsync();
        }

        public async Task<DeliveryAddress> UpdateDeliveryAddressAsync(DeliveryAddress model)
        {
           return await _repository.UpdateDeliveryAddressAsync(model);
        }
    }
}
