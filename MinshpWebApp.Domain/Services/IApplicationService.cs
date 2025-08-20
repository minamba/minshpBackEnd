using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface IApplicationService
    {
        Task<IEnumerable<Application>> GetApplicationAsync();
        Task<Application> UpdateApplicationsAsync(Application model);
        Task<Application> AddApplicationsAsync(Domain.Models.Application model);
        Task<bool> DeleteApplicationsAsync(int idApplication);
    }
}
