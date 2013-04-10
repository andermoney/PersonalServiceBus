using System;
using System.Linq;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using Raven.Client.Document;

namespace PersonalServiceBus.RSS.Infrastructure.RavenDB
{
    public class RavenDatabase : IDatabase, IDisposable
    {
        private readonly DocumentStore _documentStore;

        public RavenDatabase(IConfiguration configuration)
        {
            _documentStore = new DocumentStore();
            _documentStore.Url = configuration.RavenDBUrl;
            _documentStore.Initialize();
        }

        public void Dispose()
        {
            if (_documentStore != null)
                _documentStore.Dispose();
        }

        public IQueryable<T> Query<T>()
        {
            using (var documentSession = _documentStore.OpenSession())
            {
                return documentSession.Query<T>();
            }
        }

        public void Store<T>(T entity)
        {
            using (var documentSession = _documentStore.OpenSession())
            {
                documentSession.Store(entity);
                documentSession.SaveChanges();
            }
        }
    }
}