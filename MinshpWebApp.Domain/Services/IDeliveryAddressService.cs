using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface IDeliveryAddressService
    {
        Task<IEnumerable<DeliveryAddress>> GetDeliveryAddressesAsync();
        Task<DeliveryAddress> UpdateDeliveryAddressAsync(DeliveryAddress model);
        Task<DeliveryAddress> AddDeliveryAddresssAsync(Domain.Models.DeliveryAddress model);
        Task<bool> DeleteDeliveryAddresssAsync(int idDeliveryAddress);
    }
}
