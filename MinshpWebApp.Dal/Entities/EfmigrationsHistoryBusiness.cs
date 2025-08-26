using System;
using System.Collections.Generic;

namespace MinshpConsolApp.Models;

public partial class EfmigrationsHistoryBusiness
{
    public string MigrationId { get; set; } = null!;

    public string ProductVersion { get; set; } = null!;
}
