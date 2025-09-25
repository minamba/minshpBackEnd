using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetRole = MinshpWebApp.Domain.Models.AspNetRole;

namespace MinshpWebApp.Dal.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public RoleRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AspNetRole>> GetRolesAsync()
        {
            var RoleEntities = await _context.AspNetRoles.Select(p => new AspNetRole
            {
                Id = p.Id,
                Name = p.Name,
                NormalizedName = p.NormalizedName

            }).ToListAsync();

            return RoleEntities;
        }
    }
}
