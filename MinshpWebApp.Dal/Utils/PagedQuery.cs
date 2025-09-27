using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MinshpWebApp.Dal.Utils
{
    public class PagedQuery
    {
        public static async Task<PageResult<TOut>> ExecuteAsync<T, TOut>(
         IQueryable<T> query,
         PageRequest req,
         IEnumerable<Expression<Func<T, string?>>>? searchFields,
         IReadOnlyDictionary<string, Func<IQueryable<T>, string, IQueryable<T>>>? filterHandlers,
         string defaultSort,
         Expression<Func<T, TOut>> selector,
         int maxPageSize = 100,
         CancellationToken ct = default)
        {
            // 1) bornes
            var page = req.Page <= 0 ? 1 : req.Page;
            var pageSize = req.PageSize <= 0 ? 20 : Math.Min(req.PageSize, maxPageSize);

            // 2) search
            if (!string.IsNullOrWhiteSpace(req.Search) && searchFields != null)
                query = ApplySearch(query, req.Search!, searchFields);

            // 3) filters
            if (req.Filter?.Count > 0 && filterHandlers != null)
                foreach (var (key, val) in req.Filter)
                    if (!string.IsNullOrWhiteSpace(val) && filterHandlers.TryGetValue(key, out var apply))
                        query = apply(query, val!);

            // 4) sort
            query = ApplySorting(query, string.IsNullOrWhiteSpace(req.Sort) ? defaultSort : req.Sort!);

            // 5) total
            var total = await query.CountAsync(ct);

            // 6) page (OFFSET)
            var pageItems = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(selector)
                .ToListAsync(ct);

            return new PageResult<TOut>
            {
                Items = pageItems,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
        }

        // ---- helpers privés (génériques) ----
        private static IQueryable<T> ApplySearch<T>(IQueryable<T> q, string search, IEnumerable<Expression<Func<T, string?>>> fields)
        {
            var s = search.ToLowerInvariant();
            var p = Expression.Parameter(typeof(T), "x");
            Expression? or = null;
            foreach (var f in fields)
            {
                var body = Expression.Invoke(f, p);
                var coalesce = Expression.Coalesce(body, Expression.Constant(""));
                var lower = Expression.Call(coalesce, nameof(string.ToLower), Type.EmptyTypes);
                var contains = Expression.Call(lower, nameof(string.Contains), Type.EmptyTypes, Expression.Constant(s));
                or = or == null ? contains : Expression.OrElse(or, contains);
            }
            return or == null ? q : q.Where(Expression.Lambda<Func<T, bool>>(or, p));
        }

        private static IQueryable<T> ApplySorting<T>(IQueryable<T> q, string sort)
        {
            IOrderedQueryable<T>? ordered = null;
            foreach (var token in sort.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var parts = token.Split(':', StringSplitOptions.TrimEntries);
                var field = parts[0];
                var dir = (parts.Length > 1 ? parts[1] : "asc").ToLowerInvariant();

                var prop = typeof(T).GetProperty(field,
                    System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (prop == null) continue;

                var p = Expression.Parameter(typeof(T), "x");
                var body = Expression.Property(p, prop);
                var keySelector = Expression.Lambda(body, p);

                var method = (ordered == null
                    ? (dir == "desc" ? "OrderByDescending" : "OrderBy")
                    : (dir == "desc" ? "ThenByDescending" : "ThenBy"));

                var mi = typeof(Queryable).GetMethods()
                    .First(m => m.Name == method && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), prop.PropertyType);

                ordered = (IOrderedQueryable<T>)mi.Invoke(null, new object[] { ordered ?? q, keySelector })!;
                q = ordered;
            }
            return ordered ?? q;
        }
    }
}
