using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class NewLetterService : INewLetterService
    {

        INewLetterRepository _repository;

        public NewLetterService(INewLetterRepository repository)
        {
            _repository = repository;
        }

        public async Task<NewLetter> AddNewLettersAsync(NewLetter model)
        {
            return await _repository.AddNewLettersAsync(model);
        }

        public async Task<bool> DeleteNewLettersAsync(int idNewLetter)
        {
            return await _repository.DeleteNewLettersAsync(idNewLetter);
        }

        public async Task<IEnumerable<NewLetter>> GetNewLetterssAsync()
        {
            return await _repository.GetNewLetterssAsync();
        }

        public async Task<NewLetter> UpdateNewLettersAsync(NewLetter model)
        {
            return await _repository.UpdateNewLettersAsync(model);
        }
    }
}
