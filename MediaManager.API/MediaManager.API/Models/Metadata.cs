using System.Collections.Generic;
using MediaManager.DAL.DBEntities;

namespace MediaManager.API
{
    public  class Metadata
    {
        public List<Content> Contents { get; set; }

        public List<Domain> Domains { get; set; }

        public List<DomainContent> DomainContents { get; set; }

        private DatabaseContext ctx;

       
    }
}