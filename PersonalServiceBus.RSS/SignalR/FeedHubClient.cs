using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.SignalR
{
    public class FeedHubClient
    {
        private readonly Lazy<IHubConnectionContext> _clientsInstance = new Lazy<IHubConnectionContext>(() => GlobalHost.ConnectionManager.GetHubContext<FeedHub>().Clients);

        private IHubConnectionContext Clients
        {
            get { return _clientsInstance.Value; }
        }

        public void UpdateFeedUnreadCount(Feed feed)
        {
            Clients.All.UpdateFeedUnreadCount(feed);
        } 
    }
}