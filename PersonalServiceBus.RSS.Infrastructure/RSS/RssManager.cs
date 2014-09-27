using System;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Core.Helper;
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

                var feedItems = rssFeed.Items.ConvertAll(i => new FeedItem
                {
                    RssId = !string.IsNullOrEmpty(i.Id) ? i.Id : i.Title,
                    Title = i.Title,
                    FeedId = feed.Id,
                    Url = i.Link,
                    Author = i.Author,
                    Content = i.Content,
                    DatePublished = i.DatePublished
                });
                return ResponseBuilder.BuildCollectionResponse(feedItems, ErrorLevel.None);
            }
            catch (InvalidFeedXmlException ex)
            {
                return ResponseBuilder.BuildCollectionResponse<FeedItem>(ErrorLevel.Critical,
                    string.Format("Failed to retrieve feed at URL: {0}", feed.Url), ex);
            }
        }

        public SingleResponse<UserFeed> LookupUserFeed(UserFeed userFeed)
        {
            if (userFeed.Feed == null || string.IsNullOrEmpty(userFeed.Feed.Url))
            {
                return ResponseBuilder.BuildSingleResponse<UserFeed>(ErrorLevel.None, "");
            }
            var factory = new HttpFeedFactory();
            try
            {
                var rssFeed = factory.CreateFeed(new Uri(userFeed.Feed.Url));

                return ResponseBuilder.BuildSingleResponse(new UserFeed
                {
                    Name = rssFeed.Title,
                    Feed = userFeed.Feed
                }, ErrorLevel.None);
            }
            catch (NotSupportedException)
            {
                return ResponseBuilder.BuildSingleResponse<UserFeed>(ErrorLevel.Error, "URL type is not supported");
            }
            catch (InvalidFeedXmlException ex)
            {
                return ResponseBuilder.BuildSingleResponse<UserFeed>(ErrorLevel.Critical,
                    string.Format("Failed to retrieve feed at URL: {0}", userFeed.Feed.Url), ex);
            }
        }
    }
}