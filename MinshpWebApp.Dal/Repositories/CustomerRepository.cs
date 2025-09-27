using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Dal.Utils;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AspNetRole = MinshpWebApp.Domain.Models.AspNetRole;
using Customer = MinshpWebApp.Domain.Models.Customer;

namespace MinshpWebApp.Dal.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public CustomerRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync()
        {
            var CustomerEntities = await _context.Customers.Select(p => new Customer
            {
                Id = p.Id,
                LastName = p.LastName,
                FirstName = p.FirstName,
                PhoneNumber = p.PhoneNumber,
                Actif = p.Actif,
                BirthDate = p.BirthDate,
                Civilite = p.Civilite,
                ClientNumber = p.ClientNumber1,
                Email = p.Email,
                IdAspNetUser = p.IdAspNetUser,
                Pseudo = p.Pseudo,
            }).ToListAsync();


            foreach (var customer in CustomerEntities)
            {
                customer.Roles = await GetCustomerRolesAsync(customer.IdAspNetUser);
            }


            return CustomerEntities;
        }


        public async Task<Customer> UpdateCustomersAsync(Customer model)
        {
            var CustomerToUpdate = await _context.Customers.FirstOrDefaultAsync(u => u.IdAspNetUser == model.IdAspNetUser);

            if (CustomerToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.LastName != null) CustomerToUpdate.LastName = model.LastName;
            if (model.FirstName != null) CustomerToUpdate.FirstName = model.FirstName;
            if (model.PhoneNumber != null) CustomerToUpdate.PhoneNumber = model.PhoneNumber;
            if (model.Actif != null) CustomerToUpdate.Actif = model.Actif;
            if (model.BirthDate != null) CustomerToUpdate.BirthDate = model.BirthDate;
            if (model.Civilite != null) CustomerToUpdate.Civilite = model.Civilite;
            if (model.Email != null) CustomerToUpdate.Email = model.Email;
            if (model.Pseudo != null) CustomerToUpdate.Pseudo = model.Pseudo;



            await _context.SaveChangesAsync();


            return new Customer()
            {
                Id = model.Id,
                LastName = model.LastName,
                FirstName = model.FirstName,
                PhoneNumber = model.PhoneNumber,
                Actif = model.Actif,
                BirthDate = model.BirthDate,
                Civilite = model.Civilite,
                ClientNumber = model.ClientNumber1,
                Email = model.Email,
                IdAspNetUser = model.IdAspNetUser,
                Pseudo = model.Pseudo
            };
        }


        public async Task<Customer> AddCustomersAsync(Domain.Models.Customer model)
        {
            var newCustomer = new Dal.Entities.Customer
            {
                Id = model.Id,
                LastName = model.LastName,
                FirstName = model.FirstName,
                PhoneNumber = model.PhoneNumber,
                Actif = model.Actif,
                BirthDate = model.BirthDate,
                Civilite = model.Civilite,
                Email = model.Email.ToLower(),
                IdAspNetUser = model.IdAspNetUser,
                Pseudo = model.Pseudo
            };

            _context.Customers.Add(newCustomer);
            _context.SaveChanges();

            return new Customer()
            {
                Id = newCustomer.Id,
                LastName = model.LastName,
                FirstName = model.FirstName,
                PhoneNumber = model.PhoneNumber,
                Actif = model.Actif,
                BirthDate = model.BirthDate,
                Civilite = model.Civilite,
                ClientNumber = newCustomer.ClientNumber1,
                Email = model.Email,
                IdAspNetUser = model.IdAspNetUser,
                Pseudo = model.Pseudo
            };
        }


        public async Task<bool> DeleteCustomersAsync(int idCustomer)
        {
            var CustomerToDelete = await _context.Customers.FirstOrDefaultAsync(u => u.Id == idCustomer);
            var aspNetUserToDelete = await _context.AspNetUsers.FirstOrDefaultAsync(u => u.Id == CustomerToDelete.IdAspNetUser);

            if (CustomerToDelete == null)
                return false; // ou throw une exception;

            _context.AspNetUsers.Remove(aspNetUserToDelete);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<AspNetRole>> GetCustomerRolesAsync(string aspUid)
        {
            var user = await _context.AspNetUsers
                .AsNoTracking()
                .Include(u => u.Roles)               // <- indispensable ici
                .SingleOrDefaultAsync(u => u.Id == aspUid);

            return user?.Roles
                .Select(r => new AspNetRole
                {
                    Id = r.Id,
                    Name = r.Name!,
                    NormalizedName = r.NormalizedName
                })
                .ToList() ?? new List<AspNetRole>();

        }


        public async Task<IEnumerable<Customer>> GetCustomersByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids.Distinct().ToList();
            var customerEntities = await _context.Customers
                .AsNoTracking()
                .Where(p => idList.Contains(p.Id))
                .OrderByDescending(p => p.LastName)
                .Select(p => new Customer
                {
                    Id = p.Id,
                    LastName = p.LastName,
                    FirstName = p.FirstName,
                    PhoneNumber = p.PhoneNumber,
                    Actif = p.Actif,
                    BirthDate = p.BirthDate,
                    Civilite = p.Civilite,
                    ClientNumber = p.ClientNumber1,
                    Email = p.Email,
                    IdAspNetUser = p.IdAspNetUser,
                    Pseudo = p.Pseudo,
                })
                .ToListAsync();

            foreach (var c in customerEntities)
            {
                c.Roles = await GetCustomerRolesAsync(c.IdAspNetUser);
            }

            // Remet l'ordre des IDs paginés (important pour garder le tri)
            var customer = idList.Select((id, i) => new { id, i }).ToDictionary(x => x.id, x => x.i);
            return customerEntities.OrderBy(p => customer[p.Id]).ToList();
        }


        public async Task<PageResult<int>> PageCustomerIdsAsync(PageRequest req, CancellationToken ct = default)
        {
            var q = _context.Customers.AsNoTracking();

            // champs de recherche génériques
            var search = new Expression<Func<Dal.Entities.Customer, string?>>[]
            {
                p => p.LastName, p => p.FirstName, p => p.Pseudo, p => p.Email
            };



            // filtres génériques (clé = Filter.<Key> côté front)
            var filters = new Dictionary<string, Func<IQueryable<Dal.Entities.Customer>, string, IQueryable<Dal.Entities.Customer>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["FirstName"] = (qq, v) => string.IsNullOrWhiteSpace(v) ? qq : qq.Where(p => p.FirstName != null && p.FirstName.Equals(v, StringComparison.OrdinalIgnoreCase)),
                ["LastName"] = (qq, v) => string.IsNullOrWhiteSpace(v) ? qq : qq.Where(p => p.LastName != null && p.LastName.Equals(v, StringComparison.OrdinalIgnoreCase)),
                ["Pseudo"] = (qq, v) => string.IsNullOrWhiteSpace(v) ? qq : qq.Where(p => p.Pseudo != null && p.Pseudo.Equals(v, StringComparison.OrdinalIgnoreCase)),
                ["Email"] = (qq, v) => string.IsNullOrWhiteSpace(v) ? qq : qq.Where(p => p.Email != null && p.Email.Equals(v, StringComparison.OrdinalIgnoreCase)),

            };

            // On page d'abord sur les IDs (rapide + stable), tri par défaut
            var page = await PagedQuery.ExecuteAsync<Dal.Entities.Customer, int>(
                query: q,
                req: req,
                searchFields: search,
                filterHandlers: filters,
                defaultSort: "LastName:asc",
                selector: p => p.Id,
                ct: ct
            );

            return page;
        }
    }
}
