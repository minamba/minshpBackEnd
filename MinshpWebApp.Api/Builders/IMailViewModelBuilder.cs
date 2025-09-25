using MinshpWebApp.Api.Request;
using MinshpWebApp.Api.ViewModels;
using MinshpWebApp.Domain.Models;

namespace MinshpWebApp.Api.Builders
{
    public interface IMailViewModelBuilder
    {
        Task<string> SendMailRegistration(string mail);

        Task<string> SendMailPayment(string mail, List<Utils.InvoiceItem> items, CustomerViewModel customer, BillingAddress billing, DeliveryAddress delivery, Order order, decimal tva, decimal totalTaxes);

        Task<string> SendMailPasswordReset(string recipient, string resetLink);
        //Task<string> SendMailGroupRegistration(List<string> recipients);
    }
}
