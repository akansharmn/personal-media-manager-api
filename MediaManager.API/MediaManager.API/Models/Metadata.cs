using System;
using System.Collections.Generic;
using MediaManager.DAL.DBEntities;
using Microsoft.EntityFrameworkCore;

namespace MediaManager.API
{
    /// <summary>
    /// Class which loads metadata on startup
    /// </summary>
    public  class Metadata
    {
        /// <summary>
        /// Content types supported
        /// </summary>
        public List<Content> Contents { get; set; }

        /// <summary>
        /// Domains supported
        /// </summary>
        public List<Domain> Domains { get; set; }

        /// <summary>
        /// information regarding domain and content
        /// </summary>
        public List<DomainContent> DomainContents { get; set; }

        private DatabaseContext ctx;

        /// <summary>
        /// Mapping between table name and entity set
        /// </summary>
        public Dictionary<string, Type> TableDbSetMapping { get; set; }



       
    }
}