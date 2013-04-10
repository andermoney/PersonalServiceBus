﻿using System;
using System.Linq;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;

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

        public Status AddFeed(Feed feed)
        {
            try
            {
                _database.Store(feed);
                return new Status
                    {
                        ErrorLevel = ErrorLevel.None
                    };
            }
            catch (Exception ex)
            {
                return new Status
                    {
                        ErrorLevel = ErrorLevel.Critical,
                        ErrorMessage = string.Format("Fatal error adding feed: {0}", ex),
                        ErrorException = ex
                    };
            }
        }
    }
}