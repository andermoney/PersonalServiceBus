using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.SignalR
{
    public class FeedHubClient
    {
        private IHubConnectionContext Clients
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<FeedHub>().Clients; }
        }

        public void UpdateFeedUnreadCount(Feed feed)
        {
            Clients.All.UpdateFeedUnreadCount(feed);
        } 
    }
}