﻿namespace MinshpWebApp.Api.Options
{
    public class StripeSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string PublishableKey { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;
        public string FrontendBaseUrl { get; set; } = string.Empty; // optionnel
    }
}
