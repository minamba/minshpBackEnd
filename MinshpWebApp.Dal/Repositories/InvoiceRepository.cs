using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
