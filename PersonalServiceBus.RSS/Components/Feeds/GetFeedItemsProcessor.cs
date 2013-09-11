using System;
using System.Collections.Generic;
using System.Linq;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Messages.Feeds;
using PersonalServiceBus.RSS.SignalR;

namespace PersonalServiceBus.RSS.Components.Feeds
{
    public class GetFeedItemsProcessor
    {
        private readonly IFeedManager _feedManager;
        private readonly IRssManager _rssManager;
        private readonly IAuthentication _authentication;
        private readonly FeedHubClient _feedHubClient;

        public GetFeedItemsProcessor(IFeedManager feedManager,
            IRssManager rssManager,
            IAuthentication authentication,
            FeedHubClient feedHubClient)
        {
            _feedManager = feedManager;
            _rssManager = rssManager;
            _authentication = authentication;
            _feedHubClient = feedHubClient;
        }

        public void Handle(GetFeedItems message)
        {
            var nextFeedResponse = _feedManager.GetNextFeed();
            var nextFeed = nextFeedResponse.Data;

            if (nextFeed != null)
            {
                CollectionResponse<FeedItem> feedItemsResponse = _rssManager.GetFeedItems(nextFeed);

                if (feedItemsResponse.Status.ErrorLevel >= ErrorLevel.Error)
                {
                    nextFeed.FeedRetrieveDate = DateTime.Now;
                    nextFeed.Status = feedItemsResponse.Status;
                    nextFeed.Status.ErrorException = null; //Have to strip this out for RavenDB, does not serialize well
                    //TODO log the exception itself
                    _feedManager.UpdateFeed(nextFeed);
                }
                else
                {
                    IEnumerable<FeedItem> newFeedItems = new List<FeedItem>();
                    if (feedItemsResponse.Data.Any())
                    {
                        //Add the items for the feed and get any new ones
                        var addFeedItemsResponse = _feedManager.AddFeedItems(feedItemsResponse.Data);
                        //TODO log the error response if we're unable to save feed items
                        newFeedItems = addFeedItemsResponse.Data;
                    }

                    //Set last date feed was retrieved
                    nextFeed.FeedRetrieveDate = DateTime.Now;
                    _feedManager.UpdateFeed(nextFeed);

                    //If there are new feed items
                    if (newFeedItems.Any())
                    {
                        //Get all users subscribed to this feed
                        var userFeedsResponse = _feedManager.GetUserFeedsByUrl(nextFeed.Url);
                        foreach (var userFeed in userFeedsResponse.Data)
                        {
                            //Add the items for each feed
                            var user = new User
                            {
                                Id = userFeed.RavenUserId
                            };
                            CollectionResponse<UserFeedItem> feedItemsAddResponse = _feedManager.AddUserFeedItems(newFeedItems, user);

                            //TODO log items add issues
                            var getUserResponse = _authentication.GetUserByUserId(user);
                            if (getUserResponse.Data != null)
                            {
                                user = getUserResponse.Data;
                                SingleResponse<int> unreadCountResponse = _feedManager.GetUserFeedUnreadCount(userFeed);
                                if (unreadCountResponse.Status.ErrorLevel > ErrorLevel.Warning)
                                {
                                    userFeed.Status = unreadCountResponse.Status;
                                    userFeed.Status.ErrorException = null;
                                    _feedManager.UpdateUserFeed(userFeed);
                                }
                                else
                                {
                                    userFeed.UnreadCount = unreadCountResponse.Data;
                                    _feedManager.UpdateUserFeed(userFeed);
                                    _feedHubClient.UpdateFeedUnreadCount(user.Username, userFeed);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}