﻿using System;
using System.Collections.Generic;
using System.Linq;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using Raven.Client.Document;

namespace PersonalServiceBus.RSS.Infrastructure.RavenDB
{
    public class RavenDatabase : IDatabase, IDisposable
    {
        private readonly DocumentStore _documentStore;

        public RavenDatabase(IConfiguration configuration)
        {
            _documentStore = new DocumentStore
                {
                    Url = configuration.RavenDBUrl
                };
            _documentStore.Initialize();
        }

        public void Dispose()
        {
            if (_documentStore != null)
                _documentStore.Dispose();
        }

        public T Load<T>(string id)
        {
            using (var documentSession = _documentStore.OpenSession())
            {
                return documentSession.Load<T>(id);
            }
        }

        public IQueryable<T> Query<T>()
        {
            using (var documentSession = _documentStore.OpenSession())
            {
                return documentSession.Query<T>();
            }
        }

        public void Store<T>(T entity) where T : EntityBase
        {
            if (typeof (T) == typeof (User))
            {
                throw new InvalidOperationException("Should not be saving user.  Password must be properly encrypted");
            }
            using (var documentSession = _documentStore.OpenSession())
            {
                documentSession.Store(entity);
                documentSession.SaveChanges();
            }
        }

        public void StoreCollection<T>(IEnumerable<T> entities) where T : EntityBase
        {
            using (var documentSession = _documentStore.OpenSession())
            {
                foreach (var entity in entities)
                {
                    documentSession.Store(entity);
                }
                documentSession.SaveChanges();
            }
        }
    }
}