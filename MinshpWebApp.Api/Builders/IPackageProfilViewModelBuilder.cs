using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IPackageProfilViewModelBuilder
    {
        Task<IEnumerable<PackageProfilViewModel>> GetPackageProfilsAsync();
        Task<PackageProfil> UpdatePackageProfilsAsync(PackageProfilRequest model);
        Task<PackageProfil> AddPackageProfilsAsync(PackageProfilRequest model);
        Task<bool> DeletePackageProfilsAsync(int idPackageProfil);
    }
}
