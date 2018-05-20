using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API.Services
{
    public interface ITypeHelperService
    {
         bool TypeHasProperties<T>(string fields);
    }
}
