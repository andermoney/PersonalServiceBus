using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Messages.Feeds;
using PersonalServiceBus.RSS.SignalR;

namespace PersonalServiceBus.RSS.Components.Feeds
{
    public class GetFeedItemsProcessor : IHandleMessages<GetFeedItems>
    {
        public IBus Bus { get; set; }

        private readonly IFeedManager _feedManager;
        private readonly IRssManager _rssManager;
        private readonly FeedHubClient _feedHubClient;

        public GetFeedItemsProcessor(IFeedManager feedManager,
            IRssManager rssManager,
            FeedHubClient feedHubClient)
        {
            _feedManager = feedManager;
            _rssManager = rssManager;
            _feedHubClient = feedHubClient;
        }

        public void Handle(GetFeedItems message)
        {
            Feed nextFeed = _feedManager.GetNextFeed();

            if (nextFeed != null)
            {
                IEnumerable<FeedItem> feedItems = _rssManager.GetFeedItems(nextFeed).ToList();

                if (feedItems.Any())
                {
                    _feedManager.AddFeedItems(feedItems);
                    nextFeed.UnreadCount = _feedManager.GetFeedUnreadCount(nextFeed).Data;
                    _feedHubClient.UpdateFeedUnreadCount(nextFeed);
                }

                nextFeed.FeedRetrieveDate = DateTime.Now;
                _feedManager.UpdateFeed(nextFeed);
            }
        }
    }
}