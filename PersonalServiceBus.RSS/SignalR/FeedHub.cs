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
            var userResponse = _authentication.GetUserByUsername(new User(Context.User.Identity.Name, ""));
            var user = userResponse.Data;

            var feedResponse = _feedManager.GetFeedByUrl(feed.Url);
            if (feedResponse.Status.ErrorLevel > ErrorLevel.None)
            {
                return new SingleResponse<Feed>
                    {
                        Data = feed,
                        Status = new Status
                            {
                                ErrorLevel = feedResponse.Status.ErrorLevel,
                                ErrorMessage = string.Format("Error retrieving feed for update: {0}", feedResponse.Status.ErrorMessage)
                            }
                    };
            }
            var existingFeed = feedResponse.Data;
            //If feed doesn't exist then create it with single user subscriber
            if (existingFeed == null)
            {
                feed.UserIds = new List<string> { user.Id };
                return _feedManager.AddFeed(feed);
            }
            //If the feed does exist then check if this user is already subscribed
            if (!existingFeed.UserIds.Contains(user.Id))
            {
                //If this user hasn't subscribed to an existing feed then add them
                existingFeed.UserIds.Add(user.Id);
                return _feedManager.UpdateFeed(existingFeed);
            }
            return new SingleResponse<Feed>
                {
                    Data = feed,
                    Status = new Status
                        {
                            ErrorLevel = ErrorLevel.Information,
                            ErrorMessage = string.Format("User has already subscribed to feed at {0}", feed.Url)
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