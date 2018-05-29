using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API.Helpers
{
    /// <summary>
    /// Parameter which specifies searching, sorting, filtering and page requirement of videos serach
    /// </summary>
    public class VideoResourceParameters
    {
        /// <summary>
        /// Fields asked by user
        /// </summary>
        public string Fields { get; set; }

        /// <summary>
        /// the fields by which video have to be sorted
        /// </summary>
        public string OrderBy { get; set; } = "Name";

        /// <summary>
        /// Tiltle to be searched
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Search query to be searched
        /// </summary>
        public string searchQuery { get; set; }

        const int maxPageSize = 20;

        /// <summary>
        /// Page number to be searched
        /// </summary>
        public int pageNumber { get; set; }

        /// <summary>
        /// page number to be searched
        /// </summary>
        public int PageNumber { get; set; } = 1;

        private int pageSize = 10;

        /// <summary>
        ///  size of the page set to the minimum of maxPageSize or user provided value
        /// </summary>
        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }


    }
}
