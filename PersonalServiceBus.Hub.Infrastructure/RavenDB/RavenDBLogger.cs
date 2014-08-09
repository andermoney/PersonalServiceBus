using PersonalServiceBus.Hub.Core.Contract;
using PersonalServiceBus.Hub.Core.Domain.Interface;
using PersonalServiceBus.Hub.Core.Domain.Model;
using Raven.Client.Document;

namespace PersonalServiceBus.Hub.Infrastructure.RavenDB
{
    public class RavenDBLogger : ILogger
    {
        private readonly DocumentStore _documentStore;

        public RavenDBLogger(DocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public Response Log(LogEntry logEntry)
        {
            using (var documentSession = _documentStore.OpenSession())
            {
                documentSession.Store(logEntry);
                documentSession.SaveChanges();
            }
            return new Response();
        }
    }
}