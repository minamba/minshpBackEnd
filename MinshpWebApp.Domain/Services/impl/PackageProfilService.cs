using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class PackageProfilService : IPackageProfilService
    {
        IPackageProfilRepository _repository;


        public PackageProfilService(IPackageProfilRepository repository)
        {
            _repository = repository;
        }
        public async Task<PackageProfil> AddPackageProfilsAsync(PackageProfil model)
        {
           return await _repository.AddPackageProfilsAsync(model);
        }

        public async Task<bool> DeletePackageProfilsAsync(int idPackageProfil)
        {
            return await _repository.DeletePackageProfilsAsync(idPackageProfil);
        }

        public async Task<IEnumerable<PackageProfil>> GetPackageProfilsAsync()
        {
           return await _repository.GetPackageProfilsAsync();
        }

        public async Task<PackageProfil> UpdatePackageProfilsAsync(PackageProfil model)
        {
            return await _repository.UpdatePackageProfilsAsync(model);
        }
    }
}
