using PersonalServiceBus.RSS.Components.Feeds;
using PersonalServiceBus.RSS.Messages;
using ServiceStack;

namespace PersonalServiceBus.RSS.Services
{
    public class RSSService : Service
    {
        private readonly IFeedItemsProcessor _feedItemsProcessor;

        public RSSService(IFeedItemsProcessor feedItemsProcessor)
        {
            _feedItemsProcessor = feedItemsProcessor;
        }

        public UpdateFeedItemsResponse Post(UpdateFeedItemsRequest request)
        {
            _feedItemsProcessor.UpdateFeedItems();
            return new UpdateFeedItemsResponse();
        }
    }
}