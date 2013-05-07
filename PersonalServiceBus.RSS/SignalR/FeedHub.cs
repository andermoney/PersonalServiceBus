using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using NServiceBus;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.SignalR
{
    [HubName("feedHub")]
    public class FeedHub : Hub
    {
        public IBus Bus { get; set; }

        private readonly IFeedManager _feedManager;
        private readonly IAuthentication _authentication;

        public FeedHub()
        {
            _feedManager = Configure.Instance.Builder.Build<IFeedManager>();
            _authentication = Configure.Instance.Builder.Build<IAuthentication>();
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
            if (string.IsNullOrEmpty(Context.User.Identity.Name))
            {
                return new CollectionResponse<Feed>
                    {
                        Data = new List<Feed>(),
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Error,
                                ErrorMessage = "Please log in to view feeds"
                            }
                    };
            }

            Groups.Add(Context.ConnectionId, Context.User.Identity.Name);
            
            return _feedManager.GetFeeds();
        }
    }
}