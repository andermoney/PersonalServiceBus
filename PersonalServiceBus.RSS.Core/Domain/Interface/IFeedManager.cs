using System.Collections.Generic;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IFeedManager
    {
        Feed GetNextFeed();
        IEnumerable<Category> GetFeedCategories(out Status status);
        Status AddFeed(Feed feed);
        IEnumerable<Feed> GetFeeds(out Status status);
    }
}