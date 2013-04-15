using System;
using System.Collections.Generic;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using QDFeedParser;
using QDFeedParser.Xml;

namespace PersonalServiceBus.RSS.Infrastructure.RSS
{
    public class RssManager : IRssManager
    {
        public IEnumerable<FeedItem> GetFeedItems(Feed feed)
        {
            var factory = new HttpFeedFactory();
            var rssFeed = factory.CreateFeed(new Uri(feed.Url));

            return rssFeed.Items.ConvertAll(i => new FeedItem
                {
                    RssId = !string.IsNullOrEmpty(i.Id) ? i.Id : i.Title,
                    Title = i.Title,
                    FeedId = feed.Id,
                    Category = feed.Category,
                    Url = i.Link,
                    Author = i.Author,
                    Content = i.Content,
                    DatePublished = i.DatePublished
                });
        }
    }
}