using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API.Helpers
{
    /// <summary>
    /// Parameter which specifies searching, sorting, filtering and page requirement of users serach
    /// </summary>
    public class UserResourceParameter
    {
        /// <summary>
        /// Fields separated by comma asked by user to return
        /// </summary>
        public string Fields { get; set; }

        /// <summary>
        /// properties by which users have to be sorted
        /// </summary>
        public string OrderBy { get; set; } = "Username";

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }


        /// <summary>
        /// search query to be searched
        /// </summary>
        public string searchQuery { get; set; }

        /// <summary>
        /// maximum page size
        /// </summary>
        const int maxPageSize = 20;

        /// <summary>
        /// Page number
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// size of the page
        /// </summary>
        private int pageSize { get; set; }

        /// <summary>
        /// size of the page set to the minimum of maxPgeSize or user provided value
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
