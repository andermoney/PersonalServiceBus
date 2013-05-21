using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using NServiceBus;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Models;

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

        public override Task OnDisconnected()
        {
            var user = new User
                {
                    Username = Context.User.Identity.Name
                };
            var removeConnectionResponse = _authentication.RemoveConnection(Context.ConnectionId, user);
            //TODO log connection errors
            Groups.Remove(Context.ConnectionId, Context.User.Identity.Name);

            return base.OnDisconnected();
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

        public SingleResponse<UserFeed> AddFeed(AddFeedViewModel feedModel)
        {
            var userResponse = _authentication.GetUserByUsername(new User
                {
                    Username = Context.User.Identity.Name
                });
            var user = userResponse.Data;

            var feedResponse = _feedManager.GetFeedByUrl(feedModel.Url);
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
                var addFeedResult = _feedManager.AddFeed(new UserFeed
                    {
                        Feed = new Feed
                            {
                                Url = feedModel.Url
                            },
                        User = user,
                        Category = feedModel.Category,
                        Name = feedModel.Name
                    });
                if (addFeedResult.Status.ErrorLevel >= ErrorLevel.Error)
                {
                    return new SingleResponse<UserFeed>
                        {
                            Data = addFeedResult.Data,
                            Status = new Status
                                {
                                    ErrorLevel = addFeedResult.Status.ErrorLevel,
                                    ErrorMessage = string.Format("Error adding feed: {0}", addFeedResult.Status.ErrorMessage)
                                }
                        };
                }
                return new SingleResponse<UserFeed>
                    {
                        Data = addFeedResult.Data,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.None
                            }
                    };
            }
            //check for existing user feed
            var getUseFeedResult = _feedManager.GetUserFeedByUserId(user);
            if (getUseFeedResult != null)
            {
                return new SingleResponse<UserFeed>
                    {
                        Data = new UserFeed
                            {
                                Feed = existingFeed
                            },
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Information,
                                ErrorMessage =
                                    string.Format("User has already subscribed to feed at {0}", existingFeed.Url)
                            }
                    };
            }
            //add the user to the feed
            var addUserFeedResult = _feedManager.AddFeed(new UserFeed
                {
                    Feed = existingFeed,
                    User = user,
                    Category = feedModel.Category,
                    Name = feedModel.Name
                });
            if (addUserFeedResult.Status.ErrorLevel >= ErrorLevel.Error)
            {
                return new SingleResponse<UserFeed>
                {
                    Data = addUserFeedResult.Data,
                    Status = new Status
                    {
                        ErrorLevel = addUserFeedResult.Status.ErrorLevel,
                        ErrorMessage = string.Format("Error adding feed: {0}", addUserFeedResult.Status.ErrorMessage)
                    }
                };
            }
            return new SingleResponse<UserFeed>
            {
                Data = addUserFeedResult.Data,
                Status = new Status
                {
                    ErrorLevel = ErrorLevel.None
                }
            };
        }

        public CollectionResponse<UserFeed> GetFeeds()
        {
            if (string.IsNullOrEmpty(Context.User.Identity.Name))
            {
                return new CollectionResponse<UserFeed>
                    {
                        Data = new List<UserFeed>(),
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Error,
                                ErrorMessage = "Please log in to view feeds"
                            }
                    };
            }

            //TODO use client connection for this
            Groups.Add(Context.ConnectionId, Context.User.Identity.Name);

            var userQuery = new User
                {
                    Username = Context.User.Identity.Name
                };
            var userResponse = _authentication.GetUserByUsername(userQuery);
            if (userResponse.Status.ErrorLevel > ErrorLevel.Warning)
            {
                return new CollectionResponse<UserFeed>
                    {
                        Data = new List<UserFeed>(),
                        Status = new Status
                            {
                                ErrorLevel = userResponse.Status.ErrorLevel,
                                ErrorMessage = string.Format("Unable to retrieve feeds for user \"{0}\": {1}", userQuery.Username, userResponse.Status.ErrorMessage)
                            }
                    };
            }

            var user = userResponse.Data;
            if (user == null)
            {
                {
                    return new CollectionResponse<UserFeed>
                    {
                        Data = new List<UserFeed>(),
                        Status = new Status
                        {
                            ErrorLevel = ErrorLevel.Error,
                            ErrorMessage = "Please log in to view feeds"
                        }
                    };
                }
            }
            
            return _feedManager.GetUserFeeds(user);
        }
    }
}