using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using NServiceBus;
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
        }

        public IEnumerable<Category> GetFeedCategories()
        {
            Status status;
            return _feedManager.GetFeedCategories(out status);
        }

        public Status AddFeed(Feed feed)
        {
            return _feedManager.AddFeed(feed);
        }
    }
}