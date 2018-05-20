using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API
{
    public class MetadataStore
    {
        public static Metadata Metadata { get; set; }

        static MetadataStore()
        {
            Metadata = new Metadata();
        }

        public static void LoadEntities(DatabaseContext context)
        {

            Metadata.Contents = context.Contents.ToList();
            Metadata.Domains = context.Domains.ToList();
            Metadata.DomainContents = context.DomainContents.ToList();
        }
    }
}
