using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetInvoicesAsync();
        Task<Invoice> UpdateInvoicesAsync(Invoice model);
        Task<Invoice> AddInvoicesAsync(Domain.Models.Invoice model);
        Task<bool> DeleteInvoicesAsync(int idInvoice);
    }
}
