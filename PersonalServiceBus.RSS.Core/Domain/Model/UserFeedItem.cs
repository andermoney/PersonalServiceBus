using System;

namespace PersonalServiceBus.RSS.Core.Domain.Model
{
    public class UserFeedItem : EntityBase
    {
        public string FeedItemId { get; set; }

        public string FeedId { get; set; }

        public string RavenUserId { get; set; }

        public bool IsUnread { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}