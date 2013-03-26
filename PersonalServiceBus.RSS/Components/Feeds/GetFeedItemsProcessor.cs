using NServiceBus;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Messages.Feeds;

namespace PersonalServiceBus.RSS.Components.Feeds
{
    public class GetFeedItemsProcessor : IHandleMessages<GetFeedItems>
    {
        public IBus Bus { get; set; }

        private readonly IFeedManager _feedManager;

        public GetFeedItemsProcessor(IFeedManager feedManager)
        {
            _feedManager = feedManager;
        }

        public void Handle(GetFeedItems message)
        {
            Feed nextFeed = _feedManager.GetNextFeed();
        }
    }
}