namespace MinshpWebApp.Api.Request
{
    public sealed class PasswordResetRequest
    {
        public string To { get; set; } = default!;
        public string ResetLink { get; set; } = default!;
    }
}
