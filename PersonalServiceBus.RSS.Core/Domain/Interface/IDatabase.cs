using System.Collections.Generic;
using System.Linq;

namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IDatabase
    {
        IQueryable<T> Query<T>();
        void Store<T>(T entity);
        void StoreCollection<T>(IEnumerable<T> entities);
    }
}