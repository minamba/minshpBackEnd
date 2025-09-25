using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IInvoiceViewModelBuilder
    {
        Task<IEnumerable<InvoiceViewModel>> GetInvoicesAsync();
        Task<Invoice> UpdateInvoicesAsync(InvoiceRequest model);
        Task<Invoice> AddInvoicesAsync(InvoiceRequest model);
        Task<bool> DeleteInvoicesAsync(InvoiceRequest model);
    }
}
