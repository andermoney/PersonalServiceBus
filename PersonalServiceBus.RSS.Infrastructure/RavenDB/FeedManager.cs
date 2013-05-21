using System;
using System.Collections.Generic;
using System.Linq;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Infrastructure.RavenDB.Model;

namespace PersonalServiceBus.RSS.Infrastructure.RavenDB
{
    public class FeedManager : IFeedManager
    {
        private readonly IDatabase _database;

        public FeedManager(IDatabase database)
        {
            _database = database;
        }

        public SingleResponse<Feed> GetNextFeed()
        {
            try
            {
                var nextFeed = _database.Query<Feed>().OrderBy(f => f.FeedRetrieveDate).FirstOrDefault();
                return new SingleResponse<Feed>
                    {
                        Data = nextFeed,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.None
                            }
                    };
            }
            catch (Exception ex)
            {
                return new SingleResponse<Feed>
                    {
                        Data = new Feed(),
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Critical,
                                ErrorMessage = string.Format("Fatal error getting next feed: {0}", ex),
                                ErrorException = ex
                            }
                    };
            }
        }

        public SingleResponse<UserFeed> AddFeed(UserFeed userFeed)
        {
            try
            {
                var existingFeed = _database.Query<Feed>()
                                            .FirstOrDefault(f => f.Url == userFeed.Feed.Url);
                if (existingFeed == null)
                {
                    _database.Store(userFeed);
                    existingFeed = _database.Query<Feed>()
                                            .FirstOrDefault(f => f.Url == userFeed.Feed.Url);
                }
                userFeed.Feed = existingFeed;
                _database.Store(userFeed);
                return new SingleResponse<UserFeed>
                    {
                        Data = userFeed,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.None
                            }
                    };
            }
            catch (Exception ex)
            {
                return new SingleResponse<UserFeed>
                    {
                        Data = userFeed,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Critical,
                                ErrorMessage = string.Format("Fatal error adding feed: {0}", ex),
                                ErrorException = ex
                            }
                    };
            }
        }

        public SingleResponse<UserFeed> UpdateFeed(UserFeed userFeed)
        {
            try
            {
                _database.Store(userFeed);
                return new SingleResponse<UserFeed>
                {
                    Data = userFeed,
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.None
                    }
                };
            }
            catch (Exception ex)
            {
                return new SingleResponse<UserFeed>
                {
                    Data = userFeed,
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.Critical,
                        ErrorMessage = string.Format("Fatal error updating feed: {0}", ex),
                        ErrorException = ex
                    }
                };
            }
        }

        public CollectionResponse<UserFeed> GetUserFeeds(User user)
        {
            try
            {
                //var userFeeds = _database.QueryWithChildren<RavenUser,UserFeed>(user.Id, u => u.FeedIds);
                var userFeeds = new List<UserFeed>();
                return new CollectionResponse<UserFeed>
                    {
                        Data = userFeeds,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.None
                            }
                    };
            }
            catch (Exception ex)
            {
                return new CollectionResponse<UserFeed>
                    {
                        Data = new List<UserFeed>(),
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Critical,
                                ErrorMessage = string.Format("Fatal error getting feeds: {0}", ex),
                                ErrorException = ex
                            }
                    };
            }
        }

        public SingleResponse<UserFeed> GetUserFeedByUserId(User user)
        {
            return new SingleResponse<UserFeed>
                {
                    Data = new UserFeed(),
                    Status = new Status()
                };
        }

        public SingleResponse<Feed> GetFeedByUrl(string url)
        {
            try
            {
                return new SingleResponse<Feed>
                {
                    Data = _database.Query<Feed>()
                                    .FirstOrDefault(f => f.Url == url),
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.None
                    }
                };
            }
            catch (Exception ex)
            {
                return new SingleResponse<Feed>
                {
                    Data = new Feed(),
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.Critical,
                        ErrorMessage = string.Format("Fatal error getting feed by URL: {0}", ex),
                        ErrorException = ex
                    }
                };
            }
        }

        public CollectionResponse<FeedItem> AddFeedItems(IEnumerable<FeedItem> items)
        {
            try
            {
                var feedItems = items.ToList();
                var newFeedItems = feedItems
                    .Where(feedItem => !_database.Query<FeedItem>()
                        .Any(i => i.RssId == feedItem.RssId && i.FeedId == feedItem.FeedId))
                    .ToList();
                if (newFeedItems.Any())
                {
                    //Mark the date each item is created
                    var createdDate = DateTime.Now;
                    foreach (var newFeedItem in newFeedItems)
                    {
                        newFeedItem.CreatedDate = createdDate;
                    }

                    _database.StoreCollection(newFeedItems);
                }
                return new CollectionResponse<FeedItem>
                {
                    Data = feedItems,
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.None
                    }
                };
            }
            catch (Exception ex)
            {
                return new CollectionResponse<FeedItem>
                    {
                    Data = new List<FeedItem>(),
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.Critical,
                        ErrorMessage = string.Format("Fatal error adding feed items: {0}", ex),
                        ErrorException = ex
                    }
                };
            }
        }

        public CollectionResponse<UserFeed> GetUserFeedItems(Feed feed)
        {
            //CollectionResponse<ClientConnection> connectionsResponse = _clientCommunication.GetAllConnections();
            //var connections = connectionsResponse.Data;
            //try
            //{
            //    var userFeeds = _database.QueryWithChildren<Feed, ClientConnection>(feed.Id, u => u.);
            //    return new CollectionResponse<Feed>
            //    {
            //        Data = userFeeds,
            //        Status = new Status
            //        {
            //            ErrorLevel = ErrorLevel.None
            //        }
            //    };
            //}
            //catch (Exception ex)
            //{
            //    return new CollectionResponse<Feed>
            //    {
            //        Data = new List<Feed>(),
            //        Status = new Status
            //        {
            //            ErrorLevel = ErrorLevel.Critical,
            //            ErrorMessage = string.Format("Fatal error getting feeds: {0}", ex),
            //            ErrorException = ex
            //        }
            //    };
            //}
            return new CollectionResponse<UserFeed>
                {
                    Data = new List<UserFeed>
                        {
                            new UserFeed
                                {
                                    Feed = feed,
                                    User = new User
                                        {
                                            Username = "andermoney"
                                        },
                                    UnreadCount = new Random().Next(1, 10)
                                },
                            new UserFeed
                                {
                                    Feed = feed,
                                    User = new User
                                        {
                                            Username = "dummy"
                                        },
                                    UnreadCount = new Random().Next(11,20)
                                }
                        },
                    Status = new Status
                        {
                            ErrorLevel = ErrorLevel.None
                        }
                };
        }
    }
}