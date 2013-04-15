using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.SignalR
{
    public class FeedHubClient
    {
        private readonly FeedHub _feedHub;

        public FeedHubClient(FeedHub feedHub)
        {
            _feedHub = feedHub;
        }

        public void UpdateFeedUnreadCount(Feed feed)
        {
            _feedHub.Clients.All.UpdateFeedUnreadCount(feed);
        } 
    }
}