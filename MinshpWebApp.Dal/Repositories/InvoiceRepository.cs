using AutoMapper;
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
using Invoice = MinshpWebApp.Domain.Models.Invoice;

namespace MinshpWebApp.Dal.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private MinshpDatabaseContext _context { get; set; }
        private readonly IMapper _mapper;

        public InvoiceRepository(MinshpDatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesAsync()
        {
            var InvoiceEntities = await _context.Invoices.Select(p => new Invoice
            {
                Id = p.Id,
                CustomerId = p.CustomerId,
                InvoiceNumber = p.InvoiceNumber,
                DateCreation = p.DateCreation,
                OrderId = p.OrderId,
                Representative = p.Representative,
                InvoiceLink = p.InvoiceLink
            }).ToListAsync();

            return InvoiceEntities;
        }


        public async Task<Invoice> UpdateInvoicesAsync(Invoice model)
        {
            var InvoiceToUpdate = await _context.Invoices.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (InvoiceToUpdate == null)
                return null; // ou throw une exception

            // On met à jour ses propriétés
            if (model.CustomerId != null) InvoiceToUpdate.CustomerId = model.CustomerId;
            if (model.OrderId != null) InvoiceToUpdate.OrderId = model.OrderId;
            if (model.Representative != null) InvoiceToUpdate.Representative = model.Representative;
            if (model.InvoiceLink != null) InvoiceToUpdate.InvoiceLink = model.InvoiceLink;

            await _context.SaveChangesAsync();


            return new Invoice()
            {
                Id = model.Id,
                CustomerId = model.CustomerId,
                OrderId = model.OrderId,
                Representative = model.Representative,
                InvoiceLink = model.InvoiceLink
            };
        }


        public async Task<Invoice> AddInvoicesAsync(Domain.Models.Invoice model)
        {
            var newInvoice = new Dal.Entities.Invoice
            {
                CustomerId = model.CustomerId,
                DateCreation = DateTime.Now,
                OrderId = model.OrderId,
                Representative = "M.Shop",

            };

            _context.Invoices.Add(newInvoice);
            _context.SaveChanges();

            return new Invoice()
            {
                Id = newInvoice.Id,
                CustomerId = model.CustomerId,
                DateCreation = newInvoice.DateCreation,
                OrderId = newInvoice.OrderId,
                Representative = newInvoice.Representative,
                InvoiceNumber = newInvoice.InvoiceNumber,
                InvoiceLink = newInvoice.InvoiceLink
            };
        }


        public async Task<bool> DeleteInvoicesAsync(Invoice model)
        {
            var InvoiceToDelete = await _context.Invoices.FirstOrDefaultAsync(u => u.Id == model.Id);

            if (InvoiceToDelete == null)
                return false; // ou throw une exception;




            if (!string.IsNullOrWhiteSpace(InvoiceToDelete.InvoiceLink))
            {
                try
                {
                    if (model.HardDelete == true)
                    {

                        var path = InvoiceToDelete.InvoiceLink.Replace("\\", "/").TrimStart('/');
                        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);

                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur suppression hard de la facture  : {ex.Message}");
                }
            }




            _context.Invoices.Remove(InvoiceToDelete);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<IEnumerable<Invoice>> GetInvoicesByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids.Distinct().ToList();
            var productEntities = await _context.Invoices
                .AsNoTracking()
                .Where(p => idList.Contains(p.Id))
                .OrderByDescending(p => p.DateCreation)
                .Select(p => new Invoice
                {
                    Id = p.Id,
                    CustomerId = p.CustomerId,
                    InvoiceNumber = p.InvoiceNumber,
                    DateCreation = p.DateCreation,
                    OrderId = p.OrderId,
                    Representative = p.Representative,
                    InvoiceLink = p.InvoiceLink
                })
                .ToListAsync();

            // Remet l'ordre des IDs paginés (important pour garder le tri)
            var order = idList.Select((id, i) => new { id, i }).ToDictionary(x => x.id, x => x.i);
            return productEntities.OrderBy(p => order[p.Id]).ToList();
        }




        public async Task<PageResult<int>> PageInvoiceIdsAsync(PageRequest req, CancellationToken ct = default)
        {
            var q = _context.Invoices.AsNoTracking();

            // champs de recherche génériques (ok)
            var search = new Expression<Func<Dal.Entities.Invoice, string?>>[]
            {
        p => p.InvoiceNumber,
        p => p.Order.OrderNumber
            };

            // ✅ filtres traduisibles par EF
            var filters = new Dictionary<string, Func<IQueryable<Dal.Entities.Invoice>, string, IQueryable<Dal.Entities.Invoice>>>(StringComparer.OrdinalIgnoreCase)
            {
                ["OrderId"] = (qq, v) => int.TryParse(v, out var id) ? qq.Where(p => p.OrderId == id) : qq,
                ["CustomerId"] = (qq, v) => int.TryParse(v, out var id) ? qq.Where(p => p.CustomerId == id) : qq,

                // --- EXACT (insensible à la casse) ---
                // ["InvoiceNumber"] = (qq, v) => string.IsNullOrWhiteSpace(v)
                //     ? qq
                //     : qq.Where(p => p.InvoiceNumber != null && p.InvoiceNumber.ToLower() == v.ToLower())

                // --- CONTIENT (insensible à la casse) ---
                ["InvoiceNumber"] = (qq, v) => string.IsNullOrWhiteSpace(v)
                    ? qq
                    : qq.Where(p => p.InvoiceNumber != null && EF.Functions.Like(p.InvoiceNumber, $"%{v}%")),
            };

            // ✅ champs existants uniquement
            var page = await PagedQuery.ExecuteAsync<Dal.Entities.Invoice, int>(
                query: q,
                req: req,
                searchFields: search,
                filterHandlers: filters,
                defaultSort: "DateCreation:desc,Id:asc",
                selector: p => p.Id,
                ct: ct
            );

            return page;
        }
    }
}
