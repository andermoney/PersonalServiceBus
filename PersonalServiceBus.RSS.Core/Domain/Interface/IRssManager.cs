using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IRssManager
    {
        CollectionResponse<FeedItem> GetFeedItems(Feed feed);
        SingleResponse<UserFeed> LookupUserFeed(UserFeed userFeed);
    }
}