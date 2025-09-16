using MinshpWebApp.Api.Request;

namespace MinshpWebApp.Api.Builders
{
    public interface IMailViewModelBuilder
    {
        Task<string> SendMailRegistration(string mail);

        Task<string> SendMailPayment(string mail);
        //Task<string> SendMailGroupRegistration(List<string> recipients);
    }
}
