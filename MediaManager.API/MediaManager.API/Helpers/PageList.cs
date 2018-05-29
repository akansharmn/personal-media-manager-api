using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API.Helpers
{
    /// <summary>
    /// Class to support a paged list result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageList<T> : List<T>
    {
        /// <summary>
        /// the current page number
        /// </summary>
        public int CurrentPage { get; private set; }

        /// <summary>
        /// Total number of pages in the resultset
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Size of each page
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Toal pages
        /// </summary>
        public int TotalCount {get; private set;}


        /// <summary>
        /// Previous pages are present or not
        /// </summary>
        public bool HasPrevious
        {
            get
            {
                return (CurrentPage > 1);
            }
        }

        /// <summary>
        /// Next pages are present or not
        /// </summary>
        public bool HasNext
        {
            get
            {
                return (CurrentPage < TotalPages);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items">the source list</param>
        /// <param name="count">count of pages</param>
        /// <param name="pageNumber">page number of current page</param>
        /// <param name="pageSize">size of the page</param>
        public PageList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        /// <summary>
        /// Creates a PageList
        /// </summary>
        /// <param name="source">source list</param>
        /// <param name="pageNumber">current page number</param>
        /// <param name="pageSize">size of the page </param>
        /// <returns></returns>
        public static PageList<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {
            int count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PageList<T>(items, count, pageNumber, pageSize);
        }

       
    }
}
