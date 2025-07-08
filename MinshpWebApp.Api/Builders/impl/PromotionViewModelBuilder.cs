using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using MinshpWebApp.Domain.Services.impl;

namespace MinshpWebApp.Api.Builders.impl
{
    public class PromotionViewModelBuilder : IPromotionViewModelBuilder
    {
        private IMapper _mapper;
        private IPromotionService _promotionService;


        public PromotionViewModelBuilder(IPromotionService promotionService, IMapper mapper)
        {
            _mapper = mapper;
            _promotionService = promotionService;
        }

        public async Task<Promotion> AddPromotionsAsync(PromotionRequest model)
        {
            return await _promotionService.AddPromotionsAsync(_mapper.Map<Promotion>(model));
        }

        public async Task<bool> DeletePromotionsAsync(int idPromotion)
        {
            return await _promotionService.DeletePromotionsAsync(idPromotion);
        }

        public async Task<IEnumerable<PromotionViewModel>> GetPromotionsAsync()
        {
            var result = await _promotionService.GetPromotionsAsync();

            return  _mapper.Map<IEnumerable<PromotionViewModel>>(result);
        }

        public async Task<Promotion> UpdatePromotionsAsync(PromotionRequest model)
        {
            var promotion = _mapper.Map<Promotion>(model);
            var result = await _promotionService.UpdatePromotionsAsync(promotion);

            return result;
        }
    }
}
