namespace PersonalServiceBus.RSS.Core.Domain.Model
{
    public class UserFeed : EntityBase
    {
        public string Category { get; set; }

        public string Name { get; set; }

        public Feed Feed { get; set; }

        public User User { get; set; }

        public int UnreadCount { get; set; }
    }
}