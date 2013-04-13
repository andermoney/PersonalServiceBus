using System.Collections.Generic;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using RestSharp;
using GetRssFeed = GetRssFeed.GetRssFeed;

namespace PersonalServiceBus.RSS.Infrastructure.RSS
{
    public class RssManager : IRssManager
    {
        public IEnumerable<FeedItem> GetFeedItems(Feed feed)
        {
            var getRssFeed = new global::GetRssFeed.GetRssFeed(feed.Url, "", "");
            var request = new RestRequest();
            var feedItems = getRssFeed.Execute(request);
            return new List<FeedItem>();
        }
    }
}