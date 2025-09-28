namespace MinshpWebApp.Api.Request
{
    public class TelegramRequest
    {
        public string Mail { get; set; }
        public string ClientNumber { get; set; }
        public string OrderNumber { get; set; }
        public decimal? OrderAmount { get; set; }
        public string Date { get; set; }

        public string? Brand { get; set;}
        public string? Model { get; set; }

    }
}
