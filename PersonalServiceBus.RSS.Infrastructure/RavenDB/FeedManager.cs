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

        public Feed GetNextFeed()
        {
           var nextFeed = _database.Query<Feed>().OrderBy(f => f.FeedRetrieveDate).FirstOrDefault();
            return nextFeed;
        }

        public SingleResponse<Feed> AddFeed(Feed feed)
        {
            try
            {
                if (!_database.Query<Feed>().Any(f => f.Url == feed.Url))
                {
                    _database.Store(feed);
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
            catch (Exception ex)
            {
                return new SingleResponse<Feed>
                    {
                        Data = null,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Critical,
                                ErrorMessage = string.Format("Fatal error adding feed: {0}", ex),
                                ErrorException = ex
                            }
                    };
            }
        }

        public SingleResponse<Feed> UpdateFeed(Feed feed)
        {
            try
            {
                _database.Store(feed);
                return new SingleResponse<Feed>
                {
                    Data = feed,
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
                    Data = null,
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.Critical,
                        ErrorMessage = string.Format("Fatal error updating feed: {0}", ex),
                        ErrorException = ex
                    }
                };
            }
        }

        public CollectionResponse<Feed> GetFeeds(User user)
        {
            try
            {
                var userFeeds = _database.QueryWithChildren<RavenUser,Feed>(user.Id, u => u.FeedIds);
                return new CollectionResponse<Feed>
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
                return new CollectionResponse<Feed>
                    {
                        Data = new List<Feed>(),
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Critical,
                                ErrorMessage = string.Format("Fatal error getting feeds: {0}", ex),
                                ErrorException = ex
                            }
                    };
            }
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
                                    FeedId = feed.Id,
                                    User = new User
                                        {
                                            Username = "andermoney"
                                        },
                                    UnreadCount = new Random().Next(1, 10)
                                },
                            new UserFeed
                                {
                                    FeedId = feed.Id,
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

        public CollectionResponse<Category> GetFeedCategories()
        {
            try
            {
                return new CollectionResponse<Category>
                    {
                        Data = _database.Query<Feed>()
                                        .Select(f => f.Category)
                                        .Distinct().ToList()
                                        .Select(c => new Category
                                            {
                                                Id = c,
                                                Name = c
                                            }),
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.None
                            }
                    };
            }
            catch (Exception ex)
            {
                return new CollectionResponse<Category>
                    {
                        Data = new List<Category>(),
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Critical,
                                ErrorMessage = string.Format("Fatal error getting feed categories: {0}", ex),
                                ErrorException = ex
                            }
                    };
            }
        }
    }
}