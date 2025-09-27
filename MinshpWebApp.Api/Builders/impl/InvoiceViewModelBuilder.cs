using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;
using Org.BouncyCastle.Pqc.Crypto.Lms;

namespace MinshpWebApp.Api.Builders.impl
{
    public class InvoiceViewModelBuilder : IInvoiceViewModelBuilder
    {
        private IMapper _mapper;
        private IInvoiceService _InvoiceService;


        public InvoiceViewModelBuilder(IInvoiceService InvoiceService, IMapper mapper)
        {
            _mapper = mapper;
            _InvoiceService = InvoiceService;
        }

        public async Task<Invoice> AddInvoicesAsync(InvoiceRequest model)
        {
            return await _InvoiceService.AddInvoicesAsync(_mapper.Map<Invoice>(model));
        }

        public async Task<bool> DeleteInvoicesAsync(InvoiceRequest model)
        {
            var result = _mapper.Map<Invoice>(model);

            return await _InvoiceService.DeleteInvoicesAsync(result);
        }

        public async Task<IEnumerable<InvoiceViewModel>> GetInvoicesAsync()
        {
            var result = await _InvoiceService.GetInvoicesAsync();

            return _mapper.Map<IEnumerable<InvoiceViewModel>>(result);
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByIdsAsync(IEnumerable<int> ids)
        {
            return await _InvoiceService.GetInvoicesByIdsAsync(ids);
        }

        public async Task<PageResult<InvoiceViewModel>> PageInvoiceIdsAsync(PageRequest req, CancellationToken ct = default)
        {
            var idPage = await _InvoiceService.PageInvoiceIdsAsync(req, ct);

            if (idPage.Items.Count == 0)
            {
                return new PageResult<InvoiceViewModel>
                {
                    Items = Array.Empty<InvoiceViewModel>(),
                    TotalCount = idPage.TotalCount,
                    Page = idPage.Page,
                    PageSize = idPage.PageSize
                };
            }

            var invoices = (await _InvoiceService.GetInvoicesByIdsAsync(idPage.Items)).ToList();
            var result = _mapper.Map<List<InvoiceViewModel>>(invoices);

            // Respect de l’ordre paginé
            var order = idPage.Items.Select((id, i) => new { id, i }).ToDictionary(x => x.id, x => x.i);
            result = result.OrderBy(vm => order[vm.Id]).ToList();

            return new PageResult<InvoiceViewModel>
            {
                Items = result,
                TotalCount = idPage.TotalCount,
                Page = idPage.Page,
                PageSize = idPage.PageSize
            };
        }

        public async Task<Invoice> UpdateInvoicesAsync(InvoiceRequest model)
        {
            var Invoice = _mapper.Map<Invoice>(model);
            var result = await _InvoiceService.UpdateInvoicesAsync(Invoice);

            return result;
        }
    }
}
