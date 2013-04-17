using System.Collections.Generic;
using System.Linq;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IDatabase
    {
        IQueryable<T> Query<T>();
        void Store<T>(T entity) where T : EntityBase;
        void StoreCollection<T>(IEnumerable<T> entities) where T : EntityBase;
    }
}