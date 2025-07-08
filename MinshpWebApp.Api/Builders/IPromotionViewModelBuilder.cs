using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IPromotionViewModelBuilder
    {
        Task<IEnumerable<PromotionViewModel>> GetPromotionsAsync();
        Task<Promotion> UpdatePromotionsAsync(PromotionRequest model);
        Task<Promotion> AddPromotionsAsync(PromotionRequest model);
        Task<bool> DeletePromotionsAsync(int idPromotion);
    }
}
