﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Infrastructure.RavenDB.Model;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace PersonalServiceBus.RSS.Infrastructure.RavenDB
{
    public class RavenDatabase : IDatabase, IDisposable
    {
        protected DocumentStore DocumentStore;

        public RavenDatabase(IConfiguration configuration)
        {
            DocumentStore = new DocumentStore
            {
                Url = configuration.RavenDBUrl,
                DefaultDatabase = "RSS"
            };
            DocumentStore.Initialize();

            BuildIndexes();
        }

        public void Dispose()
        {
            if (DocumentStore != null)
                DocumentStore.Dispose();
        }

        public T Load<T>(string id)
        {
            using (var documentSession = DocumentStore.OpenSession())
            {
                return documentSession.Load<T>(id);
            }
        }

        public IQueryable<T> Query<T>()
        {
            using (var documentSession = DocumentStore.OpenSession())
            {
                return documentSession.Query<T>();
            }
        }

        public IEnumerable<TChild> QueryWithChildren<TParent,TChild>(string parentId, Expression<Func<TParent,object>> childIdCollection)
        {
            var childIdMemberExpression = childIdCollection.Body as MemberExpression;
            if (childIdMemberExpression == null)
            {
                throw new ArgumentException("childIdCollection must be a member expression");
            }
            var propertyInfo = childIdMemberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("childIdCollection must be collection of ids to query");
            }
            
            using (var documentSession = DocumentStore.OpenSession())
            {
                var result = new List<TChild>();
                var parent = documentSession.Include(childIdCollection)
                                            .Load(parentId);
                if (parent == null)
                {
                    throw new NullReferenceException("parentId returned a null object");
                }
                var childCollection = propertyInfo.GetValue(parent, null) as IEnumerable<string>;
                if (childCollection != null)
                {
                    foreach (var childId in childCollection)
                    {
                        var child = documentSession.Load<TChild>(childId);
                        result.Add(child);
                    }
                }

                return result;
            }
        }

        public IEnumerable<TResult> QueryWithIncludes<TResult>(Expression<Func<TResult, object>> path, Expression<Func<TResult, bool>> queryExpression)
        {
            using (var documentSession = DocumentStore.OpenSession())
            {
                var query = documentSession.Query<TResult>()
                    .Where(queryExpression);
                var results = new List<TResult>();
                foreach (var result in query)
                {
                    if (result is EntityBase)
                    {
                        var entity = result as EntityBase;
                        results.Add(documentSession.Include(path).Load(entity.Id));
                    }
                }
                return results;
            }
        }

        public string Store<T>(T entity) where T : EntityBase
        {
            if (typeof (T) == typeof (User))
            {
                throw new InvalidOperationException("Should not be saving user.  Password must be properly encrypted");
            }
            using (var documentSession = DocumentStore.OpenSession())
            {
                documentSession.Store(entity);
                var id = documentSession.Advanced.GetDocumentId(entity);
                documentSession.SaveChanges();
                return id;
            }
        }

        public IEnumerable<T> StoreCollection<T>(IEnumerable<T> entities) where T : EntityBase
        {
            using (var documentSession = DocumentStore.OpenSession())
            {
                var entityResult = new List<T>();
                foreach (var entity in entities)
                {
                    documentSession.Store(entity);
                    var id = documentSession.Advanced.GetDocumentId(entity);
                    entity.Id = id;
                    entityResult.Add(entity);
                }
                documentSession.SaveChanges();
                return entityResult;
            }
        }

        public void Delete<T>(T entity) where T : EntityBase
        {
            using (var documentSession = DocumentStore.OpenSession())
            {
                var entityToDelete = documentSession.Load<T>(entity.Id);
                documentSession.Delete(entityToDelete);
                documentSession.SaveChanges();
            }
        }

        private void BuildIndexes()
        {
            //DocumentStore.DatabaseCommands.PutIndex("RavenUsersByName",
            //    new IndexDefinitionBuilder<RavenUser>
            //    {
            //        Map = users => from user in users
            //            select new {user.Username}
            //    });
        }
    }
}