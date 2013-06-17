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
        IEnumerable<TChild> QueryWithChildren<TParent, TChild>(string parentId, Expression<Func<TParent, object>> childIdCollection);
        IEnumerable<TResult> QueryWithIncludes<TResult>(Expression<Func<TResult, object>> path, Expression<Func<TResult, bool>> queryExpression);
        void Store<T>(T entity) where T : EntityBase;
        void StoreCollection<T>(IEnumerable<T> entities) where T : EntityBase;
        void Delete<T>(T entity) where T : EntityBase;
    }
}