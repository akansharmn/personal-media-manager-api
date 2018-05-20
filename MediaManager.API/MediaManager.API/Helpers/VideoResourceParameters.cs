using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API.Helpers
{
    public class VideoResourceParameters
    {
        public string Fields { get; set; }
        public string OrderBy { get; set; } = "Name";
        public string Title { get; set; }

        public string searchQuery { get; set; }

        const int maxPageSize = 20;
        public int pageNumber { get; set; }

        public int PageNumber { get; set; } = 1;

        private int pageSize = 10;

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
