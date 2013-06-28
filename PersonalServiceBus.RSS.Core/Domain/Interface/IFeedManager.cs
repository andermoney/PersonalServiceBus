using System.Collections.Generic;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IFeedManager
    {
        SingleResponse<Feed> GetNextFeed();
        SingleResponse<Feed> UpdateFeed(Feed feed);

        SingleResponse<UserFeed> AddUserFeed(UserFeed userFeed);
        SingleResponse<UserFeed> UpdateUserFeed(UserFeed userFeed);
        CollectionResponse<UserFeed> GetUserFeedsByUrl(string url);
        CollectionResponse<UserFeed> GetUserFeeds(User user);
        SingleResponse<UserFeed> GetUserFeedByUserId(User user);
        SingleResponse<UserFeed> GetUserFeedByUserIdAndUrl(User user, string url);
        SingleResponse<int> GetUserFeedUnreadCount(UserFeed userFeed);

        CollectionResponse<FeedItem> AddFeedItems(IEnumerable<FeedItem> feedItems);
        CollectionResponse<UserFeedItem> AddUserFeedItems(IEnumerable<FeedItem> feedItems, User user);
    }
}