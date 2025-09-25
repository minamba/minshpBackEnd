using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<MinshpWebApp.Domain.Models.AspNetRole>> GetRolesAsync();
    }
}
