using AutoMapper;
using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;
using MinshpWebApp.Domain.Services;

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

        public async Task<bool> DeleteInvoicesAsync(int idInvoice)
        {
            return await _InvoiceService.DeleteInvoicesAsync(idInvoice);
        }

        public async Task<IEnumerable<InvoiceViewModel>> GetInvoicesAsync()
        {
            var result = await _InvoiceService.GetInvoicesAsync();

            return _mapper.Map<IEnumerable<InvoiceViewModel>>(result);
        }

        public async Task<Invoice> UpdateInvoicesAsync(InvoiceRequest model)
        {
            var Invoice = _mapper.Map<Invoice>(model);
            var result = await _InvoiceService.UpdateInvoicesAsync(Invoice);

            return result;
        }
    }
}
