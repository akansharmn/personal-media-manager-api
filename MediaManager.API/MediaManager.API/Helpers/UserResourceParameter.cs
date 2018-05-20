using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API.Helpers
{
    public class UserResourceParameter
    {
        public string Fields { get; set; }

        public string OrderBy { get; set; } = "Username";

        public string Username { get; set; }

        //public string Email { get; set; }

        //public string Name { get; set; }

        public string searchQuery { get; set; }

        const int maxPageSize = 20;

        public int PageNumber { get; set; }

        private int pageSize { get; set; }

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
