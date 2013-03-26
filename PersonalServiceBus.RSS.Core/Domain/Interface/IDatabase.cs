using System.Linq;

namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IDatabase
    {
        IQueryable<T> Query<T>();
    }
}