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

        public ApplicationRepository()
        {
            _context = new MinshpDatabaseContext();
        }

        public async Task<IEnumerable<Application>> GetApplicationAsync()
        {
            var ApplicationEntities = await _context.Applications.Select(p => new Application
            {
                Id = p.Id,
                DisplayNewProductNumber = p.DisplayNewProductNumber,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
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



            await _context.SaveChangesAsync();


            return new Application()
            {
                Id = ApplicationToUpdate.Id,
                DisplayNewProductNumber = ApplicationToUpdate.DisplayNewProductNumber,
                StartDate = ApplicationToUpdate.StartDate,
                EndDate = ApplicationToUpdate.EndDate,
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
            };

            _context.Applications.Add(newApplication);
            _context.SaveChanges();

            return new Application()
            {
                Id = newApplication.Id,
                DisplayNewProductNumber = newApplication.DisplayNewProductNumber,
                StartDate = newApplication.StartDate,
                EndDate = newApplication.EndDate,
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
