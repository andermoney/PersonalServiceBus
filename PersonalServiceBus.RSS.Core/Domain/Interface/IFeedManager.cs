using System.Collections.Generic;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IFeedManager
    {
        Feed GetNextFeed();
        CollectionResponse<Category> GetFeedCategories();
        SingleResponse<Feed> AddFeed(Feed feed);
        SingleResponse<Feed> UpdateFeed(Feed feed);
        CollectionResponse<Feed> GetFeeds();

        CollectionResponse<FeedItem> AddFeedItems(IEnumerable<FeedItem> feedItems);
    }
}