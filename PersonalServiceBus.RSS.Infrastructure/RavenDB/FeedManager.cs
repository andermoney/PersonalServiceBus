using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Helper;
using PersonalServiceBus.RSS.Infrastructure.RavenDB.Model;

namespace PersonalServiceBus.RSS.Infrastructure.RavenDB
{
    public class FeedManager : IFeedManager
    {
        private readonly IDatabase _database;

        public FeedManager(IDatabase database)
        {
            _database = database;

            Mapper.CreateMap<Feed, RavenFeed>();
            Mapper.CreateMap<UserFeed, RavenUserFeed>();
            Mapper.CreateMap<RavenFeed, Feed>();
            Mapper.CreateMap<RavenUserFeed, UserFeed>();
        }

        public SingleResponse<Feed> GetNextFeed()
        {
            try
            {
                var nextFeed = _database.Query<RavenFeed>().OrderBy(f => f.FeedRetrieveDate).FirstOrDefault();
                return new SingleResponse<Feed>
                    {
                        Data = Mapper.Map<Feed>(nextFeed),
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

        public SingleResponse<UserFeed> AddUserFeed(UserFeed userFeed)
        {
            try
            {
                if (userFeed == null)
                    return ResponseBuilder.BuildSingleResponse<UserFeed>(ErrorLevel.Error, "userFeed is required");
                if (userFeed.Feed == null)
                    return ResponseBuilder.BuildSingleResponse<UserFeed>(ErrorLevel.Error, "userFeed.Feed is required");

                var existingFeed = _database.Query<RavenFeed>()
                                            .FirstOrDefault(f => f.Url == userFeed.Feed.Url);
                string existingFeedId;
                if (existingFeed == null)
                {
                    var ravenFeed = Mapper.Map<RavenFeed>(userFeed.Feed);
                    existingFeedId = _database.Store(ravenFeed);
                }
                else
                {
                    existingFeedId = existingFeed.Id;
                }
                var ravenUserFeed = Mapper.Map<RavenUserFeed>(userFeed);
                ravenUserFeed.RavenFeedId = existingFeedId;
                _database.Store(ravenUserFeed);
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

        public SingleResponse<UserFeed> UpdateUserFeed(UserFeed userFeed)
        {
            try
            {
                if (userFeed == null)
                    return ResponseBuilder.BuildSingleResponse<UserFeed>(ErrorLevel.Error, "userFeed is required");
                if (userFeed.Feed == null)
                    return ResponseBuilder.BuildSingleResponse<UserFeed>(ErrorLevel.Error, "userFeed.Feed is required");
                if (userFeed.Feed.Id == null)
                    return ResponseBuilder.BuildSingleResponse<UserFeed>(ErrorLevel.Error, "userFeed.Feed.Id is required");

                var existingFeed = _database.Load<RavenFeed>(userFeed.Feed.Id);
                if (existingFeed == null)
                {
                    return ResponseBuilder.BuildSingleResponse(userFeed, ErrorLevel.Error, string.Format("Feed not found with id {0}", userFeed.Feed.Id));
                }
                var updateFeedResponse = UpdateFeed(userFeed.Feed);
                if (updateFeedResponse.Status.ErrorLevel > ErrorLevel.None)
                {
                    return ResponseBuilder.BuildSingleResponse(userFeed, updateFeedResponse.Status.ErrorLevel, updateFeedResponse.Status.ErrorMessage);
                }

                var ravenUserFeed = Mapper.Map<RavenUserFeed>(userFeed);
                ravenUserFeed.RavenFeedId = existingFeed.Id;
                _database.Store(ravenUserFeed);
                return ResponseBuilder.BuildSingleResponse(userFeed, ErrorLevel.None);
            }
            catch (Exception ex)
            {
                return new SingleResponse<UserFeed>
                {
                    Data = userFeed,
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.Critical,
                        ErrorMessage = string.Format("Fatal error updating user feed: {0}", ex),
                        ErrorException = ex
                    }
                };
            }
        }

        public SingleResponse<Feed> UpdateFeed(Feed feed)
        {
            try
            {
                var ravenFeed = Mapper.Map<RavenFeed>(feed);
                _database.Store(ravenFeed);
                return ResponseBuilder.BuildSingleResponse(feed, ErrorLevel.None);
            }
            catch (Exception ex)
            {
                return new SingleResponse<Feed>
                    {
                        Data = feed,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Critical,
                                ErrorMessage = string.Format("Fatal error updating feed: {0}", ex)
                            }
                    };
            }
        }

        public CollectionResponse<UserFeed> GetUserFeeds(User user)
        {
            try
            {
                if (user == null)
                    return ResponseBuilder.BuildCollectionResponse<UserFeed>(ErrorLevel.Error, "user is required");
                if (user.Id == null)
                    return ResponseBuilder.BuildCollectionResponse<UserFeed>(ErrorLevel.Error, "User Id is required");

                var ravenUserFeeds = _database.Query<RavenUserFeed>()
                    .Where(uf => uf.RavenUserId == user.Id)
                    .ToList();
                var userFeeds = new List<UserFeed>();
                foreach (var ravenUserFeed in ravenUserFeeds)
                {
                    var userFeed = Mapper.Map<UserFeed>(ravenUserFeed);
                    var feed = _database.Load<RavenFeed>(ravenUserFeed.RavenFeedId);
                    userFeed.Feed = Mapper.Map<Feed>(feed);
                    userFeeds.Add(userFeed);
                }

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

        public SingleResponse<UserFeed> GetUserFeedByUserIdAndUrl(User user, string url)
        {
            try
            {
                var ravenFeed = _database.Query<RavenFeed>().FirstOrDefault(f => f.Url == url);
                if (ravenFeed == null)
                {
                    return new SingleResponse<UserFeed>
                    {
                        Data = null,
                        Status = new Status
                        {
                            ErrorLevel = ErrorLevel.None
                        }
                    };
                }
                var ravenUserFeed = _database.Query<RavenUserFeed>()
                    .FirstOrDefault(uf => uf.RavenFeedId == ravenFeed.Id && uf.RavenUserId == user.Id);

                if (ravenUserFeed == null)
                {
                    return new SingleResponse<UserFeed>
                        {
                            Data = null,
                            Status = new Status
                                {
                                    ErrorLevel = ErrorLevel.None
                                }
                        };
                }

                var userFeed = Mapper.Map<UserFeed>(ravenUserFeed);
                userFeed.Feed = Mapper.Map<Feed>(ravenFeed);

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
                return ResponseBuilder.BuildSingleResponse<UserFeed>(ErrorLevel.Critical,
                                                                     string.Format("Fatal error getting feed by URL: {0}", ex), ex);
            }
        }

        public SingleResponse<int> GetUserFeedUnreadCount(UserFeed userFeed)
        {
            try
            {
                if (userFeed == null)
                {
                    return ResponseBuilder.BuildSingleResponse<int>(ErrorLevel.Error, "User feed is required");
                }
                if (userFeed.Feed == null)
                {
                    return ResponseBuilder.BuildSingleResponse<int>(ErrorLevel.Error, "userFeed.Feed cannot be null");
                }
                var feedId = userFeed.Feed.Id;
                var count = _database.Query<UserFeedItem>()
                         .Count(uf => uf.FeedId == feedId && 
                             uf.RavenUserId == userFeed.RavenUserId && 
                             uf.IsUnread);

                return ResponseBuilder.BuildSingleResponse(count, ErrorLevel.None);
            }
            catch (Exception ex)
            {
                return ResponseBuilder.BuildSingleResponse<int>(ErrorLevel.Critical,
                                                                     string.Format("Fatal error getting user feed unread count: {0}", ex), ex);
            }
        }

        public CollectionResponse<UserFeed> GetUserFeedsByUrl(string url)
        {
            try
            {
                var ravenFeed = _database.Query<RavenFeed>().FirstOrDefault(f => f.Url == url);
                if (ravenFeed == null)
                {
                    return ResponseBuilder.BuildCollectionResponse<UserFeed>(ErrorLevel.None, "");
                }
                var ravenUserFeeds = _database.Query<RavenUserFeed>()
                    .Where(uf => uf.RavenFeedId == ravenFeed.Id)
                    .ToList();
                var userFeeds = new List<UserFeed>();
                foreach (var ravenUserFeed in ravenUserFeeds)
                {
                    var userFeed = Mapper.Map<UserFeed>(ravenUserFeed);
                    userFeed.Feed = Mapper.Map<Feed>(ravenFeed);
                    userFeeds.Add(userFeed);
                }
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
                IEnumerable<FeedItem> newItems = new List<FeedItem>();
                if (newFeedItems.Any())
                {
                    //Mark the date each item is created
                    var createdDate = DateTime.Now;
                    foreach (var newFeedItem in newFeedItems)
                    {
                        newFeedItem.CreatedDate = createdDate;
                    }

                    newItems = _database.StoreCollection(newFeedItems);
                }
                return ResponseBuilder.BuildCollectionResponse(newItems, ErrorLevel.None);
            }
            catch (Exception ex)
            {
                return ResponseBuilder.BuildCollectionResponse<FeedItem>(ErrorLevel.Critical,
                                                                         string.Format("Fatal error adding feed items: {0}", ex));
            }
        }

        public CollectionResponse<UserFeedItem> AddUserFeedItems(IEnumerable<FeedItem> feedItems, User user)
        {
            try
            {
                var newUserFeedItems = feedItems
                    .Where(feedItem => !_database.Query<UserFeedItem>()
                        .Any(i => i.FeedItemId == feedItem.Id))
                    .Select(i => new UserFeedItem
                        {
                            FeedItemId = i.Id,
                            FeedId = i.FeedId,
                            RavenUserId = user.Id,
                            IsUnread = true
                        })
                    .ToList();
                if (newUserFeedItems.Any())
                {
                    //Mark the date each item is created
                    var createdDate = DateTime.Now;
                    foreach (var newUserFeedItem in newUserFeedItems)
                    {
                        newUserFeedItem.CreatedDate = createdDate;
                    }

                    _database.StoreCollection(newUserFeedItems);
                }
                return ResponseBuilder.BuildCollectionResponse(newUserFeedItems, ErrorLevel.None);
            }
            catch (Exception ex)
            {
                return ResponseBuilder.BuildCollectionResponse<UserFeedItem>(ErrorLevel.Critical,
                    string.Format("Fatal error adding feed items: {0}", ex), ex);
            }
        }

        public CollectionResponse<UserFeedItem> GetUserFeedItems(UserFeed userFeed)
        {
            try
            {
                if (userFeed == null)
                {
                    return ResponseBuilder.BuildCollectionResponse<UserFeedItem>(ErrorLevel.Error, "User feed is required");
                }
                if (string.IsNullOrEmpty(userFeed.RavenUserId))
                {
                    return ResponseBuilder.BuildCollectionResponse<UserFeedItem>(ErrorLevel.Error, "User Id is required");
                }
                if (userFeed.Feed == null || string.IsNullOrEmpty(userFeed.Feed.Id))
                {
                    return ResponseBuilder.BuildCollectionResponse<UserFeedItem>(ErrorLevel.Error, "Feed is required");
                }

                var feedId = userFeed.Feed.Id;
                var userFeedItems = _database.Query<UserFeedItem>()
                    .Where(ufi => ufi.RavenUserId == userFeed.RavenUserId && ufi.FeedId == feedId)
                    .ToList();

                return ResponseBuilder.BuildCollectionResponse(userFeedItems, ErrorLevel.None);
            }
            catch (Exception ex)
            {
                return ResponseBuilder.BuildCollectionResponse<UserFeedItem>(ErrorLevel.Critical,
                    string.Format("Fatal error getting feed items: {0}", ex), ex);
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
                                    RavenUserId = "ravenuser/1",
                                    UnreadCount = new Random().Next(1, 10)
                                },
                            new UserFeed
                                {
                                    Feed = feed,
                                    RavenUserId = "ravenuser/2",
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