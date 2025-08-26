using System;
using System.Collections.Generic;

namespace MinshpWebApp.Dal.Entities;

public partial class OpenIddictAuthorization
{
    public string Id { get; set; } = null!;

    public string? ApplicationId { get; set; }

    public string? ConcurrencyToken { get; set; }

    public DateTime? CreationDate { get; set; }

    public string? Properties { get; set; }

    public string? Scopes { get; set; }

    public string? Status { get; set; }

    public string? Subject { get; set; }

    public string? Type { get; set; }

    public virtual OpenIddictApplication? Application { get; set; }

    public virtual ICollection<OpenIddictToken> OpenIddictTokens { get; set; } = new List<OpenIddictToken>();
}
