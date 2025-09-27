using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Domain.Models
{
    public class PageResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
    }


    public sealed class PageRequest
    {
        public int Page { get; init; } = 1;           // 1-based
        public int PageSize { get; init; } = 20;      // cap côté service
        public string? Search { get; init; }
        public string? Sort { get; init; }            // "Field:asc,Other:desc"
        public Dictionary<string, string?> Filter { get; init; } = new();
    }
}
