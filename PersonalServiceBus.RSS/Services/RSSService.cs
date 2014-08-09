using PersonalServiceBus.RSS.Components.Feeds;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Messages;
using PersonalServiceBus.RSS.SignalR;
using ServiceStack;

namespace PersonalServiceBus.RSS.Services
{
    public class RSSService : Service
    {
        private readonly IFeedItemsProcessor _feedItemsProcessor;
        private readonly FeedHubClient _feedHubClient;
        private readonly IAuthentication _authentication;

        public RSSService(IFeedItemsProcessor feedItemsProcessor,
            FeedHubClient feedHubClient,
            IAuthentication authentication)
        {
            _feedItemsProcessor = feedItemsProcessor;
            _feedHubClient = feedHubClient;
            _authentication = authentication;
        }

        public UpdateFeedItemsResponse Post(UpdateFeedItemsRequest request)
        {
            _feedItemsProcessor.UpdateFeedItems();
            return new UpdateFeedItemsResponse();
        }

        public CheckClientsResponse Post(CheckClientsRequest request)
        {
            //_feedHubClient.CheckClients()
            return new CheckClientsResponse();
        }
    }
}