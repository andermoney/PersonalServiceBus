using System;
using System.Collections.Generic;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using QDFeedParser;

namespace PersonalServiceBus.RSS.Infrastructure.RSS
{
    public class RssManager : IRssManager
    {
        public CollectionResponse<FeedItem> GetFeedItems(Feed feed)
        {
            var factory = new HttpFeedFactory();
            try
            {
                var rssFeed = factory.CreateFeed(new Uri(feed.Url));

                return new CollectionResponse<FeedItem>
                    {
                        Data = rssFeed.Items.ConvertAll(i => new FeedItem
                            {
                                RssId = !string.IsNullOrEmpty(i.Id) ? i.Id : i.Title,
                                Title = i.Title,
                                FeedId = feed.Id,
                                Url = i.Link,
                                Author = i.Author,
                                Content = i.Content,
                                DatePublished = i.DatePublished
                            }),
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.None
                            }
                    };

            }
            catch (InvalidFeedXmlException ex)
            {
                return new CollectionResponse<FeedItem>
                    {
                        Data = new List<FeedItem>(),
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Critical,
                                ErrorMessage = string.Format("Failed to retrieve feed at URL: {0}", feed.Url),
                                ErrorException = ex
                            }
                    };
            }
        }
    }
}