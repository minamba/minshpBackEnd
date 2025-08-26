using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class BillingAddressService : IBillingAddressService
    {


        IBillingAddressRepository _repository;


        public BillingAddressService(IBillingAddressRepository repository)
        {
            _repository = repository;
        }


        public async Task<BillingAddress> AddBillingAddresssAsync(BillingAddress model)
        {
            return await _repository.AddBillingAddresssAsync(model);
        }

        public async Task<bool> DeleteBillingAddresssAsync(int idBillingAddress)
        {
           return await _repository.DeleteBillingAddresssAsync(idBillingAddress);
        }

        public async Task<IEnumerable<BillingAddress>> GetBillingAddressesAsync()
        {
            return await _repository.GetBillingAddressesAsync();
        }

        public async Task<BillingAddress> UpdateBillingAddressAsync(BillingAddress model)
        {
            return await _repository.UpdateBillingAddressAsync(model);
        }
    }
}
