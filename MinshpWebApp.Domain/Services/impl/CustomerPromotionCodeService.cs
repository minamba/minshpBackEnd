using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class CustomerPromotionCodeService : ICustomerPromotionCodeService
    {

        ICustomerPromotionCodeRepository _repository;


        public CustomerPromotionCodeService(ICustomerPromotionCodeRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerPromotionCode> AddCustomerPromotionCodesAsync(CustomerPromotionCode model)
        {
            return await _repository.AddCustomerPromotionCodesAsync(model);
        }

        public async Task<bool> DeleteCustomerPromotionCodesAsync(int idCustomerPromotionCode)
        {
            return await _repository.DeleteCustomerPromotionCodesAsync(idCustomerPromotionCode);
        }

        public async Task<IEnumerable<CustomerPromotionCode>> GetCustomerPromotionCodesAsync()
        {
           return await _repository.GetCustomerPromotionCodesAsync();
        }

        public Task<CustomerPromotionCode> UpdateCustomerPromotionCodesAsync(CustomerPromotionCode model)
        {
           return _repository.UpdateCustomerPromotionCodesAsync(model);
        }
    }
}
