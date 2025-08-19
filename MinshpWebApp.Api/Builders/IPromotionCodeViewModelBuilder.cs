using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IPromotionCodeViewModelBuilder
    {
        Task<IEnumerable<PromotionCodeViewModel>> GetPromotionCodesAsync();
        Task<PromotionCode> UpdatePromotionCodesAsync(PromotionCodeRequest model);
        Task<PromotionCode> AddPromotionCodesAsync(PromotionCodeRequest model);
        Task<bool> DeletePromotionCodesAsync(int idPromotionCode);
    }
}
