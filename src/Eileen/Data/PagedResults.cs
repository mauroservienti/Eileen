using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eileen.Data
{
    public abstract class PagedResults
    {
        public int CurrentPage { get; protected set; } 
        public int PageCount { get; protected set; } 
        public int PageSize { get; protected set; } 
        public int RowCount { get; protected set; }
 
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
 
        public PagedResults( IEnumerable<T> results, int currentPage, int pageCount, int pageSize, int rowCount)
        {
            Results = results;
            CurrentPage = currentPage;
            PageCount = pageCount;
            PageSize = pageSize;
            RowCount = rowCount;
        }
    }
}