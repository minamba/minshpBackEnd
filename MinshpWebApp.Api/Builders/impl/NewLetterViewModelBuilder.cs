using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;

namespace MinshpWebApp.Api.Builders.impl
{
    public class NewLetterViewModelBuilder : INewLetterViewModelBuilder
    {
        private IMapper _mapper;
        private INewLetterService _newLetterService;


        public NewLetterViewModelBuilder(INewLetterService newLetterService, IMapper mapper)
        {
            _newLetterService = newLetterService;
            _mapper = mapper;
        }

        public async Task<(string message, bool find)> AddNewLettersAsync(NewLetterRequest model)
        {

            var mailToGet = (await _newLetterService.GetNewLetterssAsync()).FirstOrDefault(m => m.Mail == model.Mail.ToLower());

            if (mailToGet != null)
                return ("Vous êtes déja abonné à la newsletter 😅", true);
            else
            {
                var newSubscriber = _mapper.Map<NewLetter>(model);
                await _newLetterService.AddNewLettersAsync(newSubscriber);
                return ("Merci pour votre abonnement à la newsletter 😁", false);
            }

            return ("Erreur lors de l'inscription à la newsletter", false);
        }

        public async Task<bool> DeleteNewLettersAsync(int idNewLetter)
        {
            return await _newLetterService.DeleteNewLettersAsync(idNewLetter);
        }


        public async Task<IEnumerable<NewLetterViewModel>> GetNewLetterssAsync()
        {
            var NewLetters = await _newLetterService.GetNewLetterssAsync();

            return _mapper.Map<IEnumerable<NewLetterViewModel>>(NewLetters);
        }


        public async Task<NewLetter> UpdateNewLettersAsync(NewLetterRequest model)
        {
            var NewLetter = _mapper.Map<NewLetter>(model);

            return await _newLetterService.UpdateNewLettersAsync(NewLetter);
        }

    }
}
