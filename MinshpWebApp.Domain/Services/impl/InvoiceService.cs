using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services.impl
{
    public class InvoiceService : IInvoiceService
    {
        IInvoiceRepository _repository;

        public InvoiceService(IInvoiceRepository repository)
        {
            _repository = repository;
        }

        public async Task<Invoice> AddInvoicesAsync(Invoice model)
        {
            return await _repository.AddInvoicesAsync(model);
        }

        public async Task<bool> DeleteInvoicesAsync(Invoice model)
        {
            return await _repository.DeleteInvoicesAsync(model);
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesAsync()
        {
           return await _repository.GetInvoicesAsync();
        }

        public async Task<Invoice> UpdateInvoicesAsync(Invoice model)
        {
           return await _repository.UpdateInvoicesAsync(model);
        }
    }
}
