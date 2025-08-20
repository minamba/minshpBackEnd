using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;

namespace MinshpWebApp.Api.Builders.impl
{
    public class ApplicationViewModelBuilder : IApplicationViewModelBuilder
    {
        private IMapper _mapper;
        private IApplicationService _applicationService;


        public ApplicationViewModelBuilder(IApplicationService applicationService, IMapper mapper)
        {
            _mapper = mapper;
            _applicationService = applicationService;
        }

        public async Task<Application> AddApplicationsAsync(ApplicationRequest model)
        {
            return await _applicationService.AddApplicationsAsync(_mapper.Map<Application>(model));
        }

        public async Task<bool> DeleteApplicationsAsync(int idApplication)
        {
            return await _applicationService.DeleteApplicationsAsync(idApplication);
        }

        public async Task<IEnumerable<ApplicationViewModel>> GetApplicationAsync()
        {
            var result = await _applicationService.GetApplicationAsync();

            return _mapper.Map<IEnumerable<ApplicationViewModel>>(result);
        }

        public async Task<Application> UpdateApplicationsAsync(ApplicationRequest model)
        {
            var application = _mapper.Map<Application>(model);
            var result = await _applicationService.UpdateApplicationsAsync(application);

            return result;
        }
    }
}
