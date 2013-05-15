using System.Collections.Generic;
using System.Threading.Tasks;
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
        private IClientCommunication _clientCommunication;

        public FeedHub()
        {
            _feedManager = Configure.Instance.Builder.Build<IFeedManager>();
            _authentication = Configure.Instance.Builder.Build<IAuthentication>();
            _clientCommunication = Configure.Instance.Builder.Build<IClientCommunication>();
        }

        public CollectionResponse<Category> GetFeedCategories()
        {
            return _feedManager.GetFeedCategories();
        }

        public override Task OnConnected()
        {
            _clientCommunication.AddConnection(new ClientConnection
                {
                    Username = Context.User.Identity.Name,
                    ConnectionIds = new List<string> { Context.ConnectionId }
                });
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            _clientCommunication.RemoveConnection(new ClientConnection
                {
                    Username = Context.User.Identity.Name,
                    ConnectionIds = new List<string> { Context.ConnectionId }
                });
            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            _clientCommunication.UpdateConnection(new ClientConnection
                {
                    Username = Context.User.Identity.Name,
                    ConnectionIds = new List<string> { Context.ConnectionId }
                });
            return base.OnReconnected();
        }

        public SingleResponse<Feed> AddFeed(Feed feed)
        {
            var userResponse = _authentication.GetUserByUsername(new User
                {
                    Username = Context.User.Identity.Name
                });
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
            //If feed doesn't exist then create it
            if (existingFeed == null)
            {
                var addFeedResult = _feedManager.AddFeed(feed);
                if (addFeedResult.Status.ErrorLevel >= ErrorLevel.Error)
                {
                    return new SingleResponse<Feed>
                        {
                            Data = addFeedResult.Data,
                            Status = new Status
                                {
                                    ErrorLevel = addFeedResult.Status.ErrorLevel,
                                    ErrorMessage = string.Format("Error adding feed: {0}", addFeedResult.Status.ErrorMessage)
                                }
                        };
                }
                existingFeed = addFeedResult.Data;
            }
            //Make sure the user has a feed collection property
            if (user.FeedIds == null)
            {
                user.FeedIds = new List<string>();
            }
            //Add the user to the feed
            if (!user.FeedIds.Contains(existingFeed.Id))
            {
                user.FeedIds.Add(existingFeed.Id);
                var userUpdateResponse = _authentication.UpdateUser(user);
                if (userUpdateResponse.Status.ErrorLevel >= ErrorLevel.Error)
                {
                    return new SingleResponse<Feed>
                        {
                            Data = existingFeed,
                            Status = new Status
                                {
                                    ErrorLevel = userUpdateResponse.Status.ErrorLevel,
                                    ErrorMessage = string.Format("Error updating user {0}: {1}", user.Username, userUpdateResponse.Status.ErrorMessage)
                                }
                        };
                }
                return new SingleResponse<Feed>
                    {
                        Data = existingFeed,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.None
                            }
                    };
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

            //TODO use client connection for this
            Groups.Add(Context.ConnectionId, Context.User.Identity.Name);

            var userQuery = new User
                {
                    Username = Context.User.Identity.Name
                };
            var userResponse = _authentication.GetUserByUsername(userQuery);
            if (userResponse.Status.ErrorLevel > ErrorLevel.Warning)
            {
                return new CollectionResponse<Feed>
                    {
                        Data = new List<Feed>(),
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
            }
            
            return _feedManager.GetFeeds(user);
        }
    }
}