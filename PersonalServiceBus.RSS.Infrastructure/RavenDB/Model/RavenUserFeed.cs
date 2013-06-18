using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Infrastructure.RavenDB.Model
{
    public class RavenUserFeed : EntityBase
    {
        public string Category { get; set; }

        public string Name { get; set; }

        public string RavenFeedId { get; set; }

        public string RavenUserId { get; set; }

        public int UnreadCount { get; set; }
    }
}