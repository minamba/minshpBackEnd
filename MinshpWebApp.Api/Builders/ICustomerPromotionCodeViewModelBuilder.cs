using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface ICustomerPromotionCodeViewModelBuilder
    {
        Task<IEnumerable<CustomerPromotionCodeViewModel>> GetCustomerPromotionCodesAsync();
        Task<CustomerPromotionCode> UpdateCustomerPromotionCodesAsync(CustomerPromotionCodeRequest model);
        Task<CustomerPromotionCode> AddCustomerPromotionCodesAsync(CustomerPromotionCodeRequest model);
        Task<bool> DeleteCustomerPromotionCodesAsync(int idCustomerPromotionCode);
    }
}
