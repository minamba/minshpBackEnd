using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class ApplicationService : IApplicationService
    {

        IApplicationRepository _repository;


        public ApplicationService(IApplicationRepository repository)
        {
            _repository = repository;
        }
        public async Task<Application> AddApplicationsAsync(Application model)
        {
           return await _repository.AddApplicationsAsync(model);
        }

        public async Task<bool> DeleteApplicationsAsync(int idApplication)
        {
           return await _repository.DeleteApplicationsAsync(idApplication);
        }

        public async Task<IEnumerable<Application>> GetApplicationAsync()
        {
           return await _repository.GetApplicationAsync();
        }

        public async Task<Application> UpdateApplicationsAsync(Application model)
        {
            return await _repository.UpdateApplicationsAsync(model);
        }
    }
}
