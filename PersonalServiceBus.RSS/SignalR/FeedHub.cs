using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using NServiceBus;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.SignalR
{
    [HubName("feedHub")]
    public class FeedHub : Hub
    {
        public IBus Bus { get; set; }

        private readonly IFeedManager _feedManager;

        public FeedHub()
        {
            _feedManager = Configure.Instance.Builder.Build<IFeedManager>();
            Configure.Instance.Configurer.RegisterSingleton<FeedHub>(this);
        }

        public CollectionResponse<Category> GetFeedCategories()
        {
            return _feedManager.GetFeedCategories();
        }

        public SingleResponse<Feed> AddFeed(Feed feed)
        {
            return _feedManager.AddFeed(feed);
        }

        public CollectionResponse<Feed> GetFeeds()
        {
            return _feedManager.GetFeeds();
        }
    }
}