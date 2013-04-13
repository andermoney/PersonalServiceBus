using System.Collections.Generic;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IRssManager
    {
        IEnumerable<FeedItem> GetFeedItems(Feed feed);
    }
}