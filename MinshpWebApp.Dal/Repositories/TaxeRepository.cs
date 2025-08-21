using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taxe = MinshpWebApp.Domain.Models.Taxe;

namespace MinshpWebApp.Dal.Repositories
{
    public class TaxeRepository : ITaxeRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public TaxeRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Taxe>> GetTaxesAsync()
        {
            var TaxeEntities = await _context.Taxes.Select(p => new Taxe
            {
                Id = p.Id,
                Name = p.Name,
                Amount = p.Amount,
                Purcentage = p.Purcentage
            }).ToListAsync();

            return TaxeEntities;
        }


        public async Task<Taxe> UpdateTaxeAsync(Taxe model)
        {
            var TaxeToUpdate = await _context.Taxes.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (TaxeToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.Name != null) TaxeToUpdate.Name = model.Name;
            if (model.Amount != null) TaxeToUpdate.Amount = model.Amount;
            if (model.Purcentage != null) TaxeToUpdate.Purcentage = model.Purcentage;


            await _context.SaveChangesAsync();


            return new Taxe()
            {
                Id = model.Id,
                Name = model.Name,
                Amount = model.Amount,
                Purcentage = model.Purcentage,
            };
        }


        public async Task<Taxe> AddTaxeAsync(Domain.Models.Taxe model)
        {
            var newTaxe = new Dal.Entities.Taxe
            {
                Id = model.Id,
                Name = model.Name,
                Amount = model.Amount,
                Purcentage = model.Purcentage,
            };

            _context.Taxes.Add(newTaxe);
            _context.SaveChanges();

            return new Taxe()
            {
                Id = model.Id,
                Name = model.Name,
                Amount = model.Amount,
                Purcentage = model.Purcentage,
            };
        }


        public async Task<bool> DeleteTaxeAsync(int idTaxe)
        {
            var TaxeToDelete = await _context.Taxes.FirstOrDefaultAsync(u => u.Id == idTaxe);

            if (TaxeToDelete == null)
                return false; // ou throw une exception;

            _context.Taxes.Remove(TaxeToDelete);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
