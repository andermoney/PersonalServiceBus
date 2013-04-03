using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using NServiceBus;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Infrastructure;
using PersonalServiceBus.RSS.Models;

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
            return new List<Category>
                {
                    new Category
                        {
                            CategoryId = 1,
                            CategoryName = "Webcomics"
                        }
                };
        }

        public void AddFeed(Feed feed)
        {
            var status = _feedManager.AddFeed(feed);
        }
    }
}