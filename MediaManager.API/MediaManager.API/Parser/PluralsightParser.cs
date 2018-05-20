using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API
{
    public class PluralsightParser : IParser
    {
        public VideoForCreationDTO video { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string DomainName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IParser Create()
        {
            throw new NotImplementedException();
        }

        public bool IsMatch(string domain)
        {
            throw new NotImplementedException();
        }

        public bool IsValid(string properties)
        {
            throw new NotImplementedException();
        }

        public bool Update(string propertyname, string value)
        {
            throw new NotImplementedException();
        }
    }
}
