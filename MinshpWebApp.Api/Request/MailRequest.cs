namespace MinshpWebApp.Api.Request
{
    public class MailRequest
    {
        public CustomerRequest? Customer { get; set; }
        public OrderRequest? Order { get; set; }
    }
}
