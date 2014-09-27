using System.Collections.Generic;
using System.Linq;
using PersonalServiceBus.Hub.Core.Contract;
using PersonalServiceBus.Hub.Core.Domain.Interface;
using PersonalServiceBus.Hub.Core.Domain.Model;
using Raven.Client.Document;
using Raven.Client.Linq;

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

        public Response GetLogs(PagedRequest request)
        {
            using (var documentSession = _documentStore.OpenSession())
            {
                return new Response<List<LogEntry>>
                {
                    Data = documentSession.Query<LogEntry>()
                        .Skip(request.Page*request.PageSize)
                        .Take(request.PageSize)
                        .OrderByDescending(l => l.CreatedDate)
                        .ToList()
                };
            }
        }
    }
}