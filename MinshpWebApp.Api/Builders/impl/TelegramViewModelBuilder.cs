using MinshpWebApp.Api.Request;

namespace MinshpWebApp.Api.Builders.impl
{
    public class TelegramViewModelBuilder : ITelegramViewModelBuilder
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;


        public TelegramViewModelBuilder(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<HttpResponseMessage> SendErrorMessage(TelegramRequest request)
        {
            var botToken = _config["TelegramError:BotToken"];
            var chatId = _config["TelegramError:GroupChatId"];

            var message = $"❌ *Problème lors du paiement d'une commande* :\n" +
                          $"- Email : {request.Mail}\n" +
                          $"- N°Client : {request.ClientNumber}\n" +
                          $"- Montant : {request.OrderAmount} €\n" +
                          $"- Date Commande : {request.Date}\n";

            var url = $"https://api.telegram.org/bot{botToken}/sendMessage";

            var payload = new Dictionary<string, string>
        {
            { "chat_id", chatId },
            { "text", message },
            { "parse_mode", "Markdown" }
        };

            return  await _httpClient.PostAsync(url, new FormUrlEncodedContent(payload));
        }

        public async Task<HttpResponseMessage> SendSuccesMessage(TelegramRequest request)
        {
            var botToken = _config["TelegramSuccess:BotToken"];
            var chatId = _config["TelegramSuccess:GroupChatId"];

            var message = $"💰 *Nouvelle commande* :\n" +
                          $"- N°Client : {request.ClientNumber}\n" +
                          $"- N°Commande : {request.OrderNumber}\n" +
                          $"- Email : {request.Mail}\n" +
                          $"- Amount : {request.OrderAmount} €\n" +
                          $"- Date : {request.Date}\n";

            var url = $"https://api.telegram.org/bot{botToken}/sendMessage";

            var payload = new Dictionary<string, string>
        {
            { "chat_id", chatId },
            { "text", message },
            { "parse_mode", "Markdown" }
        };

            return await _httpClient.PostAsync(url, new FormUrlEncodedContent(payload));
        }
    }
}
