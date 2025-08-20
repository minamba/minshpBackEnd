using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IApplicationViewModelBuilder
    {
        Task<IEnumerable<ApplicationViewModel>> GetApplicationAsync();
        Task<Application> UpdateApplicationsAsync(ApplicationRequest model);
        Task<Application> AddApplicationsAsync(ApplicationRequest model);
        Task<bool> DeleteApplicationsAsync(int idApplication);
    }
}
