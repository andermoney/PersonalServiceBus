using System.Linq;
using PersonalServiceBus.Hub.Core.Contract;
using PersonalServiceBus.Hub.Core.Domain.Interface;
using PersonalServiceBus.Hub.Core.Domain.Model;
using Raven.Client.Document;
using Raven.Client.Linq;

namespace PersonalServiceBus.Hub.Infrastructure.JsonClient
{
    public class JsonPublisher : IPublisher
    {
        private readonly DocumentStore _documentStore;
        private readonly ILogger _logger;

        public JsonPublisher(DocumentStore documentStore,
            ILogger logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        public Response Publish(HubEvent hubEvent)
        {
            using (var documentSession = _documentStore.OpenSession())
            {
                var subscriptions = documentSession.Query<Subscription>()
                    .Where(s => s.Publisher == hubEvent.Name && !string.IsNullOrEmpty(s.Host))
                    .ToList()
                    .GroupBy(s => s.Host);
                foreach (var subscriptionGroup in subscriptions)
                {
                    _logger.Log(new LogEntry
                    {
                        Host = subscriptionGroup.Key,
                        Source = "JsonPublisher",
                        Description = string.Format("Would publish {0} to {1}", string.Join(", ", subscriptionGroup.ToList().ConvertAll(s => s.Publisher)), subscriptionGroup.Key)
                    });
                }
            }
            return new Response();
        }
    }
}