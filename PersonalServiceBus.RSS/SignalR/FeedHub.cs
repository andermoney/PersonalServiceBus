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
            var addFeedResponse = _feedManager.AddFeed(feed);
            if (addFeedResponse.Status.ErrorLevel > ErrorLevel.None)
            {
                return new SingleResponse<Feed>
                    {
                        Data = addFeedResponse.Data,
                        Status = new Status
                            {
                                ErrorLevel = addFeedResponse.Status.ErrorLevel,
                                ErrorMessage = string.Format("Error adding feed: {0}", addFeedResponse.Status.ErrorMessage)
                            }
                    };
            }
            var userFeed = new UserFeed
                {
                    FeedId = addFeedResponse.Data.Id,
                    Username = Context.User.Identity.Name
                };
            var addUserFeedResponse = _feedManager.AddUserFeed(userFeed);
            if (addUserFeedResponse.Status.ErrorLevel > ErrorLevel.None)
            {
                return new SingleResponse<Feed>
                    {
                        Data = addFeedResponse.Data,
                        Status = new Status
                            {
                                ErrorLevel = addUserFeedResponse.Status.ErrorLevel,
                                ErrorMessage = string.Format("Error adding user feed: {0}", addUserFeedResponse.Status.ErrorMessage)
                            }
                    };
            }
            return new SingleResponse<Feed>
                {
                    Data = feed,
                    Status = new Status
                        {
                            ErrorLevel = ErrorLevel.None
                        }
                };
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