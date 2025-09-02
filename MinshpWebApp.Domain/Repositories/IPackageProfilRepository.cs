using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Repositories
{
    public interface IPackageProfilRepository
    {
        Task<IEnumerable<PackageProfil>> GetPackageProfilsAsync();
        Task<PackageProfil> UpdatePackageProfilsAsync(PackageProfil model);
        Task<PackageProfil> AddPackageProfilsAsync(Domain.Models.PackageProfil model);
        Task<bool> DeletePackageProfilsAsync(int idPackageProfil);
    }
}
