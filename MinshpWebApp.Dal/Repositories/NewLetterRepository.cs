using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewLetter = MinshpWebApp.Domain.Models.NewLetter;

namespace MinshpWebApp.Dal.Repositories
{
    public class NewLetterRepository : INewLetterRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public NewLetterRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NewLetter>> GetNewLetterssAsync()
        {
            var NewLetterEntities = await _context.NewLetters.Select(p => new NewLetter
            {
                Id = p.Id,
                Mail = p.Mail,
                Suscribe = p.Suscribe
            }).ToListAsync();

            return NewLetterEntities;
        }



        public async Task<NewLetter> UpdateNewLettersAsync(NewLetter model)
        {
            var NewLetterToUpdate = await _context.NewLetters.FirstOrDefaultAsync(u => u.Mail == model.OldMAil);

            if (NewLetterToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.Mail != null) NewLetterToUpdate.Mail= model.Mail;
            if (model.Suscribe != null) NewLetterToUpdate.Suscribe = model.Suscribe;

            await _context.SaveChangesAsync();


            return new NewLetter()
            {
                Id = model.Id,
                Mail = model.Mail,
                Suscribe = model.Suscribe
            };
        }


        public async Task<NewLetter> AddNewLettersAsync(Domain.Models.NewLetter model)
        {
            var newNewLetter = new Dal.Entities.NewLetter
            {
                Id = model.Id,
                Mail = model.Mail.ToLower(),
                Suscribe = model.Suscribe
            };

            _context.NewLetters.Add(newNewLetter);
            _context.SaveChanges();

            return new NewLetter()
            {
                Id = newNewLetter.Id,
                Mail = newNewLetter.Mail,
                Suscribe = newNewLetter.Suscribe
            };
        }


        public async Task<bool> DeleteNewLettersAsync(int idNewLetter)
        {
            var NewLetterToDelete = await _context.NewLetters.FirstOrDefaultAsync(u => u.Id == idNewLetter);

            if (NewLetterToDelete == null)
                return false; // ou throw une exception;

            _context.NewLetters.Remove(NewLetterToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
