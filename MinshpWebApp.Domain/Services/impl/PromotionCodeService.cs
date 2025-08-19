using AutoMapper;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class PromotionCodeService : IPromotionCodeService
    {
        private IMapper _mapper;
        private IPromotionCodeRepository _repository;

        public PromotionCodeService(IPromotionCodeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PromotionCode> AddPromotionCodesAsync(PromotionCode model)
        {
            return await _repository.AddPromotionCodesAsync(model);
        }

        public async Task<bool> DeletePromotionCodesAsync(int idPromotionCode)
        {
            return await _repository.DeletePromotionCodesAsync(idPromotionCode);
        }

        public async Task<IEnumerable<PromotionCode>> GetPromotionCodesAsync()
        {
            return await _repository.GetPromotionCodesAsync();
        }

        public async Task<PromotionCode> UpdatePromotionCodesAsync(PromotionCode model)
        {
            return await _repository.UpdatePromotionCodesAsync(model);
        }
    }
}
