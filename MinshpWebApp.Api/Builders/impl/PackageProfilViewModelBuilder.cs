using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using PackageProfil = MinshpWebApp.Domain.Models.PackageProfil;

namespace MinshpWebApp.Api.Builders.impl
{
    public class PackageProfilViewModelBuilder : IPackageProfilViewModelBuilder
    {
        private IMapper _mapper;
        private IPackageProfilService _PackageProfilService;


        public PackageProfilViewModelBuilder(IPackageProfilService PackageProfilService, IMapper mapper)
        {
            _mapper = mapper;
            _PackageProfilService = PackageProfilService;
        }

        public async Task<PackageProfil> AddPackageProfilsAsync(PackageProfilRequest model)
        {
            var newPackageProfil = _mapper.Map<PackageProfil>(model);

            return await _PackageProfilService.AddPackageProfilsAsync(newPackageProfil);
        }

        public async Task<bool> DeletePackageProfilsAsync(int idPackageProfil)
        {
            return await _PackageProfilService.DeletePackageProfilsAsync(idPackageProfil);
        }

        public async Task<IEnumerable<PackageProfilViewModel>> GetPackageProfilsAsync()
        {
            var PackageProfils = await _PackageProfilService.GetPackageProfilsAsync();
            return _mapper.Map<IEnumerable<PackageProfilViewModel>>(PackageProfils);
        }

        public async Task<PackageProfil> UpdatePackageProfilsAsync(PackageProfilRequest model)
        {
            var PackageProfil = _mapper.Map<PackageProfil>(model);

            return await _PackageProfilService.UpdatePackageProfilsAsync(PackageProfil);
        }
    }
}
