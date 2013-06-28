namespace PersonalServiceBus.RSS.Core.Domain.Model
{
    public class UserFeed : EntityBase
    {
        public string Category { get; set; }

        public string Name { get; set; }

        public Feed Feed { get; set; }

        public string RavenUserId { get; set; }

        public int UnreadCount { get; set; }

        public Status Status { get; set; }
    }
}