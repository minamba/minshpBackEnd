using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application = MinshpWebApp.Domain.Models.Application;
namespace MinshpWebApp.Dal.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public ApplicationRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Application>> GetApplicationAsync()
        {
            var ApplicationEntities = await _context.Applications.Select(p => new Application
            {
                Id = p.Id,
                DisplayNewProductNumber = p.DisplayNewProductNumber,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                DefaultDropOffChronoPost = p.DefaultDropOffChronoPost,
                DefaultDropOffMondialRelay = p.DefaultDropOffMondialRelay,
                DefaultDropOffUps = p.DefaultDropOffUps,
                DefaultDropLaposte = p.DefaultDropLaposte
            }).ToListAsync();

            return ApplicationEntities;
        }


        public async Task<Application> UpdateApplicationsAsync(Application model)
        {
            var ApplicationToUpdate = await _context.Applications.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (ApplicationToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.StartDate != null) ApplicationToUpdate.StartDate = model.StartDate;
            if (model.EndDate != null) ApplicationToUpdate.EndDate = model.EndDate;
            if (model.DisplayNewProductNumber != null) ApplicationToUpdate.DisplayNewProductNumber = model.DisplayNewProductNumber;
            if (model.DefaultDropOffChronoPost != null) ApplicationToUpdate.DefaultDropOffChronoPost = model.DefaultDropOffChronoPost;
            if (model.DefaultDropOffMondialRelay != null) ApplicationToUpdate.DefaultDropOffMondialRelay = model.DefaultDropOffMondialRelay;
            if (model.DefaultDropOffUps != null) ApplicationToUpdate.DefaultDropOffUps = model.DefaultDropOffUps;
            if (model.DefaultDropLaposte != null) ApplicationToUpdate.DefaultDropLaposte = model.DefaultDropLaposte;



            await _context.SaveChangesAsync();


            return new Application()
            {
                Id = ApplicationToUpdate.Id,
                DisplayNewProductNumber = ApplicationToUpdate.DisplayNewProductNumber,
                StartDate = ApplicationToUpdate.StartDate,
                EndDate = ApplicationToUpdate.EndDate,
                DefaultDropOffChronoPost = ApplicationToUpdate.DefaultDropOffChronoPost,
                DefaultDropOffMondialRelay = ApplicationToUpdate.DefaultDropOffMondialRelay,
                DefaultDropOffUps = ApplicationToUpdate.DefaultDropOffUps,
                DefaultDropLaposte = ApplicationToUpdate.DefaultDropLaposte,
            };
        }


        public async Task<Application> AddApplicationsAsync(Domain.Models.Application model)
        {
            var newApplication = new Dal.Entities.Application
            {
                Id = model.Id,
                DisplayNewProductNumber = model.DisplayNewProductNumber,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                DefaultDropOffChronoPost = model.DefaultDropOffChronoPost,
                DefaultDropOffMondialRelay = model.DefaultDropOffMondialRelay,
                DefaultDropOffUps = model.DefaultDropOffUps,
                DefaultDropLaposte = model.DefaultDropLaposte,
            };

            _context.Applications.Add(newApplication);
            _context.SaveChanges();

            return new Application()
            {
                Id = newApplication.Id,
                DisplayNewProductNumber = newApplication.DisplayNewProductNumber,
                StartDate = newApplication.StartDate,
                EndDate = newApplication.EndDate,
                DefaultDropOffChronoPost = newApplication.DefaultDropOffChronoPost,
                DefaultDropOffMondialRelay = newApplication.DefaultDropOffMondialRelay,
                DefaultDropOffUps = newApplication.DefaultDropOffUps,
                DefaultDropLaposte = newApplication.DefaultDropLaposte,
            };
        }


        public async Task<bool> DeleteApplicationsAsync(int idApplication)
        {
            var ApplicationToDelete = await _context.Applications.FirstOrDefaultAsync(u => u.Id == idApplication);

            if (ApplicationToDelete == null)
                return false; // ou throw une exception;

            _context.Applications.Remove(ApplicationToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
