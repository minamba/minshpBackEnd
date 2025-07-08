using AutoMapper;
using MinshpWebApp.Domain.Dtos;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class PromotionService : IPromotionService
    {
        private IMapper _mapper;
        private IPromotionRepository _repository;

        public PromotionService(IPromotionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Promotion> AddPromotionsAsync(Promotion model)
        {
            return await _repository.AddPromotionsAsync(model);
        }

        public async Task<bool> DeletePromotionsAsync(int idPromotion)
        {
            return await _repository.DeletePromotionsAsync(idPromotion);
        }

        public async Task<IEnumerable<Promotion>> GetPromotionsAsync()
        {
            return await _repository.GetPromotionsAsync();
        }

        public async Task<Promotion> UpdatePromotionsAsync(Promotion model)
        {
            return await _repository.UpdatePromotionsAsync(model);
        }
    }
}
