using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Repositories
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetInvoicesAsync();
        Task<Invoice> UpdateInvoicesAsync(Invoice model);
        Task<Invoice> AddInvoicesAsync(Domain.Models.Invoice model);
        Task<bool> DeleteInvoicesAsync(Domain.Models.Invoice model);
    }
}
