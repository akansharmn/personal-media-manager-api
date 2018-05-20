using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.DAL.DBEntities;

namespace MediaManager.API
{
    public interface IParser
    {
        VideoForCreationDTO video { get; set; }
        string DomainName { set; get; }
        
        bool IsValid(string properties);
        bool Update(string propertyname, string value);
        bool IsMatch(string domain);
        IParser Create();
    }
}
