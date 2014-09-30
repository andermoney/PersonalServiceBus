using PersonalServiceBus.RSS.Components.Feeds;
using PersonalServiceBus.RSS.Messages;
using PersonalServiceBus.RSS.SignalR;
using ServiceStack;

namespace PersonalServiceBus.RSS.Services
{
    public class RSSService : Service
    {
        private readonly IFeedItemsProcessor _feedItemsProcessor;
        private readonly FeedHubClient _feedHubClient;

        public RSSService(IFeedItemsProcessor feedItemsProcessor,
            FeedHubClient feedHubClient)
        {
            _feedItemsProcessor = feedItemsProcessor;
            _feedHubClient = feedHubClient;
        }

        public UpdateFeedItemsResponse Post(UpdateFeedItemsRequest request)
        {
            _feedItemsProcessor.UpdateFeedItems();
            return new UpdateFeedItemsResponse();
        }

        public CheckClientsResponse Post(CheckClientsRequest request)
        {
            _feedHubClient.CheckClientConnections();
            return new CheckClientsResponse();
        }
    }
}