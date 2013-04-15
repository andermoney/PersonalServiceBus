using System;
using System.Collections.Generic;
using System.Linq;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Core.Contract;

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

        public CollectionResponse<Feed> GetFeeds()
        {
            try
            {
                return new CollectionResponse<Feed>
                    {
                        Data = _database.Query<Feed>()
                                        .Select(f => f)
                                        .ToList(),
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

        public SingleResponse<int> GetFeedUnreadCount(Feed feed)
        {
            try
            {
                return new SingleResponse<int>
                {
                    Data = _database.Query<FeedItem>()
                            .Count(f => f.FeedId == feed.Id),
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.None
                    }
                };
            }
            catch (Exception ex)
            {
                return new SingleResponse<int>
                {
                    Data = -1,
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.Critical,
                        ErrorMessage = string.Format("Fatal error getting feed unread count: {0}", ex),
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
                        .Any(i => i.RssId == feedItem.RssId && i.FeedId == feedItem.FeedId)).ToList();

                if (newFeedItems.Any())
                {
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