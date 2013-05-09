using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IDatabase
    {
        T Load<T>(string id);
        IQueryable<T> Query<T>();
        void Store<T>(T entity) where T : EntityBase;
        void StoreCollection<T>(IEnumerable<T> entities) where T : EntityBase;
        IEnumerable<TChild> QueryWithChildren<TParent,TChild>(string parentId, Expression<Func<TParent,object>> childIdCollection);
    }
}