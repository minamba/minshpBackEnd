using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PackageProfil = MinshpWebApp.Domain.Models.PackageProfil;

namespace MinshpWebApp.Dal.Repositories
{
    public class PackageProfilRepository : IPackageProfilRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public PackageProfilRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PackageProfil>> GetPackageProfilsAsync()
        {
            var PackageProfilEntities = await _context.PackageProfils.Select(p => new PackageProfil
            {
                Id = p.Id,
                Description = p.Description,
                Height = p.Height,
                Longer = p.Longer,
                Weight = p.Weight,
                Width = p.Width,
                Name = p.Name
            }).ToListAsync();

            return PackageProfilEntities;
        }


        public async Task<PackageProfil> UpdatePackageProfilsAsync(PackageProfil model)
        {
            var PackageProfilToUpdate = await _context.PackageProfils.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (PackageProfilToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.Description != null) PackageProfilToUpdate.Description = model.Description;
            if (model.Height != null) PackageProfilToUpdate.Height = model.Height;
            if (model.Longer != null) PackageProfilToUpdate.Longer = model.Longer;
            if (model.Height != null) PackageProfilToUpdate.Height = model.Height;
            if (model.Width != null) PackageProfilToUpdate.Width = model.Width;
            if (model.Name != null) PackageProfilToUpdate.Name = model.Name;
            if (model.Weight != null) PackageProfilToUpdate.Weight = model.Weight;

            await _context.SaveChangesAsync();


            return new PackageProfil()
            {
                Id = model.Id,
                Description = model.Description,
                Height = model.Height,
                Longer = model.Longer,
                Weight = model.Weight,
                Width = model.Width,
                Name = model.Name
            };
        }


        public async Task<PackageProfil> AddPackageProfilsAsync(Domain.Models.PackageProfil model)
        {
            var newPackageProfil = new Dal.Entities.PackageProfil
            {
                Description = model.Description,
                Height = model.Height,
                Longer = model.Longer,
                Weight = model.Weight,
                Width = model.Width,
                Name = model.Name
            };

            _context.PackageProfils.Add(newPackageProfil);
            _context.SaveChanges();

            return new PackageProfil()
            {
                Id = newPackageProfil.Id,
                Description = model.Description,
                Height = model.Height,
                Longer = model.Longer,
                Weight = model.Weight,
                Width = model.Width,
                Name = model.Name
            };
        }


        public async Task<bool> DeletePackageProfilsAsync(int idPackageProfil)
        {
            var PackageProfilToDelete = await _context.PackageProfils.FirstOrDefaultAsync(u => u.Id == idPackageProfil);

            if (PackageProfilToDelete == null)
                return false; // ou throw une exception;

            _context.PackageProfils.Remove(PackageProfilToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
