using MinshpWebApp.Api.Utils;

namespace MinshpWebApp.Api.Request
{
    public class MailRequest
    {
        public CustomerRequest? Customer { get; set; }
        public OrderRequest? Order { get; set; }

        public string? Mail { get; set; }

        public List<InvoiceItem> ?Items { get; set; }
    }
}
