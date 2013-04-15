using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IRssManager _rssManager;

        public GetFeedItemsProcessor(IFeedManager feedManager,
            IRssManager rssManager)
        {
            _feedManager = feedManager;
            _rssManager = rssManager;
        }

        public void Handle(GetFeedItems message)
        {
            Feed nextFeed = _feedManager.GetNextFeed();

            IEnumerable<FeedItem> feedItems = _rssManager.GetFeedItems(nextFeed).ToList();

            if (feedItems.Any())
            {
                _feedManager.AddFeedItems(feedItems);
            }

            nextFeed.FeedRetrieveDate = DateTime.Now;
            _feedManager.UpdateFeed(nextFeed);
        }
    }
}