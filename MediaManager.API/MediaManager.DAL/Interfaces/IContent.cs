using System;
using System.Collections.Generic;
using System.Text;

namespace MediaManager.DAL.DBEntities
{
     public interface IContent
    {
         int ContentId { get; set; }

        int DomainId { get; set; }

       
       string Title { get; set; }

       
        string Url { get; set; }
    }
}
