using MinshpWebApp.Domain.Dtos;
using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface IPromotionService
    {
        Task<IEnumerable<Promotion>> GetPromotionsAsync();
        Task<Promotion> UpdatePromotionsAsync(Promotion model);
        Task<Promotion> AddPromotionsAsync(Promotion model);
        Task<bool> DeletePromotionsAsync(int idPromotion);
    }
}
