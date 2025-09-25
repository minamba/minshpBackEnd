using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLetter = MinshpWebApp.Domain.Models.NewLetter;

namespace MinshpWebApp.Domain.Repositories
{
    public interface INewLetterRepository
    {
        Task<IEnumerable<NewLetter>> GetNewLetterssAsync();
        Task<NewLetter> UpdateNewLettersAsync(NewLetter model);

        Task<NewLetter> AddNewLettersAsync(Domain.Models.NewLetter model);

        Task<bool> DeleteNewLettersAsync(int idNewLetter);
    }
}
