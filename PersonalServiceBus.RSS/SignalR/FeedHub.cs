using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Ninject;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Core.Helper;
using PersonalServiceBus.RSS.Infrastructure;
using PersonalServiceBus.RSS.Models;

namespace PersonalServiceBus.RSS.SignalR
{
    [HubName("feedHub")]
    public class FeedHub : Hub
    {
        private readonly IFeedManager _feedManager;
        private readonly IAuthentication _authentication;
        private readonly IRssManager _rssManager;

        public FeedHub()
        {
            _feedManager = NinjectRegistry.GetKernel().Get<IFeedManager>();
            _authentication = NinjectRegistry.GetKernel().Get<IAuthentication>();
            _rssManager = NinjectRegistry.GetKernel().Get<IRssManager>();
        }

        public override Task OnConnected()
        {
            var user = new User
                {
                    Username = Context.User.Identity.Name
                };

            var addConnectionResponse = _authentication.AddConnection(Context.ConnectionId, user);
            //TODO log connection errors
            Groups.Add(Context.ConnectionId, Context.User.Identity.Name);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var user = new User
            {
                Username = Context.User.Identity.Name
            };
            var removeConnectionResponse = _authentication.RemoveConnection(Context.ConnectionId, user);
            //TODO log connection errors
            Groups.Remove(Context.ConnectionId, Context.User.Identity.Name);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            var user = new User
                {
                    Username = Context.User.Identity.Name
                };
            var updateConnectionResponse = _authentication.UpdateConnection(Context.ConnectionId, user);
            //TODO log connection errors

            return base.OnReconnected();
        }

        public SingleResponse<UserFeed> LookupUserFeed(AddFeedViewModel feedModel)
        {
            return _rssManager.LookupUserFeed(new UserFeed
            {
                Feed = new Feed
                {
                    Url = feedModel.Url
                }
            });
        }

        public SingleResponse<UserFeed> AddFeed(AddFeedViewModel feedModel)
        {
            var userResponse = _authentication.GetUserByUsername(new User
                {
                    Username = Context.User.Identity.Name
                });
            var user = userResponse.Data;

            var feedResponse = _feedManager.GetUserFeedByUserIdAndUrl(user, feedModel.Url);
            if (feedResponse.Status.ErrorLevel > ErrorLevel.None)
            {
                return new SingleResponse<UserFeed>
                    {
                        Data = new UserFeed
                        {
                            Feed = new Feed
                                {
                                    Url = feedModel.Url
                                }
                        },
                        Status = new Status
                            {
                                ErrorLevel = feedResponse.Status.ErrorLevel,
                                ErrorMessage = string.Format("Error retrieving feed for update: {0}", feedResponse.Status.ErrorMessage)
                            }
                    };
            }
            var existingFeed = feedResponse.Data;
            //If feed doesn't exist then create it
            if (existingFeed == null)
            {
                var addFeedResult = _feedManager.AddUserFeed(new UserFeed
                    {
                        Feed = new Feed
                            {
                                Url = feedModel.Url
                            },
                        RavenUserId = user.Id,
                        Category = feedModel.Category,
                        Name = feedModel.Name
                    });
                if (addFeedResult.Status.ErrorLevel >= ErrorLevel.Error)
                {
                    return ResponseBuilder.BuildSingleResponse(addFeedResult.Data,
                        addFeedResult.Status.ErrorLevel,
                        string.Format("Error adding feed: {0}", addFeedResult.Status.ErrorMessage));
                }
                return ResponseBuilder.BuildSingleResponse(addFeedResult.Data, ErrorLevel.None);
            }
            //check for existing user feed
            var getUserFeedResult = _feedManager.GetUserFeedByUserId(user);
            if (getUserFeedResult != null)
            {
                return ResponseBuilder.BuildSingleResponse(existingFeed, ErrorLevel.Information,
                    string.Format("User has already subscribed to feed at {0}", existingFeed.Feed.Url));
            }
            //add the user to the feed
            var addUserFeedResult = _feedManager.AddUserFeed(new UserFeed
                {
                    Feed = existingFeed.Feed,
                    RavenUserId = user.Id,
                    Category = feedModel.Category,
                    Name = feedModel.Name
                });
            if (addUserFeedResult.Status.ErrorLevel >= ErrorLevel.Error)
            {
                return ResponseBuilder.BuildSingleResponse(addUserFeedResult.Data,
                    addUserFeedResult.Status.ErrorLevel,
                    string.Format("Error adding feed: {0}", addUserFeedResult.Status.ErrorMessage));
            }
            return ResponseBuilder.BuildSingleResponse(addUserFeedResult.Data, ErrorLevel.None);
        }

        public CollectionResponse<FeedCategory> GetFeeds()
        {
            if (string.IsNullOrEmpty(Context.User.Identity.Name))
            {
                return ResponseBuilder.BuildCollectionResponse<FeedCategory>(ErrorLevel.Error, "Please log in to view feeds");
            }

            //TODO use client connection for this
            Groups.Add(Context.ConnectionId, Context.User.Identity.Name);

            var userQuery = new User
                {
                    Username = Context.User.Identity.Name
                };
            var userResponse = _authentication.GetUserByUsername(userQuery);
            if (userResponse.Status.ErrorLevel > ErrorLevel.Warning || userResponse.Data == null)
            {
                return ResponseBuilder.BuildCollectionResponse<FeedCategory>(userResponse.Status.ErrorLevel,
                    string.Format("Unable to retrieve feeds for user \"{0}\": {1}", userQuery.Username,
                        userResponse.Status.ErrorMessage));
            }

            var user = userResponse.Data;
            return _feedManager.GetUserFeedsGroupedByCategory(user);
        }

        public CollectionResponse<FeedItem> GetFeedItems(UserFeed userFeed)
        {
            if (string.IsNullOrEmpty(Context.User.Identity.Name))
            {
                return ResponseBuilder.BuildCollectionResponse<FeedItem>(ErrorLevel.Error, "Please log in to view feed items");
            }

            //TODO use client connection for this
            Groups.Add(Context.ConnectionId, Context.User.Identity.Name);

            var userQuery = new User
            {
                Username = Context.User.Identity.Name
            };
            var userResponse = _authentication.GetUserByUsername(userQuery);
            if (userResponse.Status.ErrorLevel > ErrorLevel.Warning || userResponse.Data == null)
            {
                return ResponseBuilder.BuildCollectionResponse<FeedItem>(userResponse.Status.ErrorLevel,
                    string.Format("Unable to retrieve feed items for user \"{0}\": {1}", userQuery.Username, userResponse.Status.ErrorMessage));
            }

            var user = userResponse.Data;
            if (user.Id != userFeed.RavenUserId)
            {
                return ResponseBuilder.BuildCollectionResponse<FeedItem>(ErrorLevel.Error, "Please log in to view feed items");
            }

            return _feedManager.GetUserFeedItems(userFeed);
        } 
    }
}