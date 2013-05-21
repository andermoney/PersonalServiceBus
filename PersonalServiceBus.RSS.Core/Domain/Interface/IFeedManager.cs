﻿using System.Collections.Generic;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IFeedManager
    {
        //TODO make this include a status
        SingleResponse<Feed> GetNextFeed();
        SingleResponse<UserFeed> AddFeed(UserFeed userFeed);
        SingleResponse<UserFeed> UpdateFeed(UserFeed userFeed);
        SingleResponse<Feed> GetFeedByUrl(string url);

        CollectionResponse<FeedItem> AddFeedItems(IEnumerable<FeedItem> feedItems);
        CollectionResponse<UserFeed> GetUserFeeds(User user);
        SingleResponse<UserFeed> GetUserFeedByUserId(User user);
    }
}