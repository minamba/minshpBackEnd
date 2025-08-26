using System;
using System.Collections.Generic;

namespace MinshpConsolApp.Models;

public partial class EfmigrationsHistoryAuth
{
    public string MigrationId { get; set; } = null!;

    public string ProductVersion { get; set; } = null!;
}
