using MinshpWebApp.Api.ViewModels;

namespace MinshpWebApp.Api.Builders
{
    public interface IRoleViewModelBuilder
    {
        Task<IEnumerable<AspNetRoleViewModel>> GetRolesAsync();
    }
}
