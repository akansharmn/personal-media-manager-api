using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API.Repository
{
    public interface IRepository<T>
    {
        T GetEntity(string user, int id);

        List<T> GetEntities(string user);

        bool DeleteEntity(T entity);

        T PutEntity(T entity);

        T UpdateEntity(T entity);

        T PostEntity(T entity);

        bool EntityExists(int id);
    }
}
