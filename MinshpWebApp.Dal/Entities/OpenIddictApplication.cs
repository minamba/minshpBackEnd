using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class OpenIddictApplication
{
    public string Id { get; set; } = null!;

    public string? ApplicationType { get; set; }

    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }

    public string? ClientType { get; set; }

    public string? ConcurrencyToken { get; set; }

    public string? ConsentType { get; set; }

    public string? DisplayName { get; set; }

    public string? DisplayNames { get; set; }

    public string? JsonWebKeySet { get; set; }

    public string? Permissions { get; set; }

    public string? PostLogoutRedirectUris { get; set; }

    public string? Properties { get; set; }

    public string? RedirectUris { get; set; }

    public string? Requirements { get; set; }

    public string? Settings { get; set; }

    public virtual ICollection<OpenIddictAuthorization> OpenIddictAuthorizations { get; set; } = new List<OpenIddictAuthorization>();

    public virtual ICollection<OpenIddictToken> OpenIddictTokens { get; set; } = new List<OpenIddictToken>();
}
