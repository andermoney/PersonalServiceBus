namespace PersonalServiceBus.RSS.Core.Domain.Model
{
    public class UserFeed
    {
        public string FeedId { get; set; }

        public User User { get; set; }

        public int UnreadCount { get; set; }
    }
}