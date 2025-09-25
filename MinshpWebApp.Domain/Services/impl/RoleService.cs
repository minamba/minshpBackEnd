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
    public class RoleService : IRoleService
    {
        private IMapper _mapper;
        private IRoleRepository _repository;


        public RoleService(IRoleRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<IEnumerable<AspNetRole>> GetRolesAsync()
        {
           return await _repository.GetRolesAsync();
        }

    }
}
