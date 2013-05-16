using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Messages.Feeds;
using PersonalServiceBus.RSS.SignalR;

namespace PersonalServiceBus.RSS.Components.Feeds
{
    public class GetFeedItemsProcessor : IHandleMessages<GetFeedItems>
    {
        public IBus Bus { get; set; }

        private readonly IFeedManager _feedManager;
        private readonly IRssManager _rssManager;
        private readonly FeedHubClient _feedHubClient;

        public GetFeedItemsProcessor(IFeedManager feedManager,
            IRssManager rssManager,
            FeedHubClient feedHubClient)
        {
            _feedManager = feedManager;
            _rssManager = rssManager;
            _feedHubClient = feedHubClient;
        }

        public void Handle(GetFeedItems message)
        {
            Feed nextFeed = _feedManager.GetNextFeed();

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
                        //Update the feed items for any connected users
                        var userFeedItemsResponse = _feedManager.GetUserFeedItems(nextFeed);

                        foreach (var userFeed in userFeedItemsResponse.Data)
                        {
                            _feedHubClient.UpdateFeedUnreadCount(userFeed);
                        }
                    }
                }
            }
        }
    }
}