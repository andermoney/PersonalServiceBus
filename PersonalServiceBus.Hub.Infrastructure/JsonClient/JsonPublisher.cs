using System;
using System.Linq;
using PersonalServiceBus.Hub.Core.Contract;
using PersonalServiceBus.Hub.Core.Domain.Enum;
using PersonalServiceBus.Hub.Core.Domain.Interface;
using PersonalServiceBus.Hub.Core.Domain.Model;
using Raven.Client.Document;
using Raven.Client.Linq;
using ServiceStack;

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
                    .Where(s => s.Publisher == hubEvent.Name && !string.IsNullOrEmpty(s.Host) && !string.IsNullOrEmpty(s.Destination))
                    .ToList()
                    .GroupBy(s => s.Host);
                foreach (var subscriptionGroup in subscriptions)
                {
                    string host = subscriptionGroup.Key;
                    _logger.Log(new LogEntry
                    {
                        Host = host,
                        Source = "JsonPublisher",
                        Description = string.Format("Would publish {0} to {1}", string.Join(", ", subscriptionGroup.ToList().ConvertAll(s => s.Publisher)), host)
                    });
                    foreach (var subscription in subscriptionGroup)
                    {
                        string url = "http://" + host;
                        try
                        {
                            var client = new JsonServiceClient(url);
                            client.Post<Response>("/" + subscription.Destination, new LogEntry
                            {
                                Source = "Publish",
                                Host = host,
                                Description = "Event published from publisher",
                                ErrorLevel = ErrorLevel.None
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.Log(new LogEntry
                            {
                                Host = host,
                                Source = "JsonPublisher",
                                ErrorLevel = ErrorLevel.Critical,
                                Description = string.Format("Failed to publish event to {0}: {1}", url, ex)
                            });
                        }
                    }
                }
            }
            return new Response();
        }
    }
}