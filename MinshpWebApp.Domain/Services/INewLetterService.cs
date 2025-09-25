using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface INewLetterService
    {
        Task<IEnumerable<NewLetter>> GetNewLetterssAsync();
        Task<NewLetter> UpdateNewLettersAsync(NewLetter model);

        Task<NewLetter> AddNewLettersAsync(Domain.Models.NewLetter model);

        Task<bool> DeleteNewLettersAsync(int idNewLetter);
    }
}
