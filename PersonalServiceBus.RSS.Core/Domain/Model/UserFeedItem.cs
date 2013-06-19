using System;

namespace PersonalServiceBus.RSS.Core.Domain.Model
{
    public class UserFeedItem : EntityBase
    {
        public string FeedItemId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}