using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Services;
using MinshpWebApp.Domain.Services.impl;

namespace MinshpWebApp.Api.Builders.impl
{
    public class RoleViewModelBuilder : IRoleViewModelBuilder
    {
        private IMapper _mapper;
        private IRoleService _roleService;

        public RoleViewModelBuilder(IRoleService roleService, IMapper mapper)
        {
            _mapper = mapper;
            _roleService = roleService;
        }


        public async Task<IEnumerable<AspNetRoleViewModel>> GetRolesAsync()
        {
            var roles = await _roleService.GetRolesAsync();

            return _mapper.Map<IEnumerable<AspNetRoleViewModel>>(roles);
        }
    }
}
