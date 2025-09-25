using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface INewLetterViewModelBuilder
    {
        Task<IEnumerable<NewLetterViewModel>> GetNewLetterssAsync();
        Task<NewLetter> UpdateNewLettersAsync(NewLetterRequest model);

        Task<(string message,bool find)> AddNewLettersAsync(NewLetterRequest model);

        Task<bool> DeleteNewLettersAsync(int idNewLetter);
    }
}
