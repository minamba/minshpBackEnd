using Microsoft.AspNetCore.Mvc;
using MinshpWebApp.Api.Request;

namespace MinshpWebApp.Api.Builders
{
    public interface ITelegramViewModelBuilder
    {
        Task<HttpResponseMessage> SendErrorMessage(TelegramRequest user);
        Task<HttpResponseMessage> SendSuccesMessage(TelegramRequest user);
        Task<HttpResponseMessage> SendStockAlertMessage(TelegramRequest request);
    }
}
