using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface IPromotionCodeService
    {
        Task<IEnumerable<PromotionCode>> GetPromotionCodesAsync();
        Task<PromotionCode> UpdatePromotionCodesAsync(PromotionCode model);
        Task<PromotionCode> AddPromotionCodesAsync(Domain.Models.PromotionCode model);
        Task<bool> DeletePromotionCodesAsync(int idPromotionCode);
    }
}
