using PersonalServiceBus.Hub.Core.Contract;
using PersonalServiceBus.Hub.Core.Domain.Interface;
using PersonalServiceBus.Hub.Core.Domain.Model;
using Raven.Client.Document;

namespace PersonalServiceBus.Hub.Infrastructure.RavenDB
{
    public class RavenDBSubscriber : ISubscriber
    {
        private readonly DocumentStore _documentStore;

        public RavenDBSubscriber(DocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public Response Subscribe(Subscription subscription)
        {
            using (var documentSession = _documentStore.OpenSession())
            {
                documentSession.Store(subscription);
                documentSession.SaveChanges();
            }
            return new Response();
        }
    }
}