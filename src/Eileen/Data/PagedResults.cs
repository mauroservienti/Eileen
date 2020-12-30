using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eileen.Data
{
    public abstract class PagedResults
    {
        public int Page { get; protected set; } 
        public int PagesCount { get; protected set; } 
        public int PageSize { get; protected set; } 
        public int TotalItemsCount { get; protected set; }
 
        // public int FirstRowOnPage
        // {
        //     get { return (CurrentPage - 1) * PageSize + 1; }
        // }
        //
        // public int LastRowOnPage
        // {
        //     get { return Math.Min(CurrentPage * PageSize, RowCount); }
        // }
    }
 
    public class PagedResults<T> : PagedResults where T : class
    {
        public IEnumerable<T> Results { get; }
 
        public PagedResults(IEnumerable<T> results, int page, int pageSize, int pagesCount, int totalItemsCount)
        {
            Results = results;
            Page = page;
            PageSize = pageSize;
            PagesCount = pagesCount;
            TotalItemsCount = totalItemsCount;
        }
    }
}