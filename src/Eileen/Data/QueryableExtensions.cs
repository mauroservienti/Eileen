using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Eileen.Data
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResults<T>> ToPagedResultsAsync<T>(this IQueryable<T> query, int page, int pageSize) where T : class
        {
            var skip = (page - 1) * pageSize;
            var results = await query.Skip(skip).Take(pageSize).ToListAsync();
            var pageCount = (int)Math.Ceiling((double)results.Count / pageSize);

            var result = new PagedResults<T>(results, page, pageCount, pageSize, results.Count);
            return result;
        }
    }
}