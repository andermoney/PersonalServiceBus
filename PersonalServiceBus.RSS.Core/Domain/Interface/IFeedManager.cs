﻿using System.Collections.Generic;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IFeedManager
    {
        Feed GetNextFeed();
        Status AddFeed(Feed feed);
        IEnumerable<Category> GetFeedCategories(out Status status);
    }
}