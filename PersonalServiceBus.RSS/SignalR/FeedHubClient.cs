using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.SignalR
{
    public class FeedHubClient
    {
        private IHubConnectionContext<dynamic> Clients
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<FeedHub>().Clients; }
        }

        public void UpdateFeedUnreadCount(string username, UserFeed userFeed)
        {
            Clients.Group(username).UpdateFeedUnreadCount(userFeed);
        }

        public User CheckClients(User user)
        {
            var badConnections = new List<string>();
            foreach (var connection in user.ConnectionIds)
            {
                var connected = Clients.Client(connection).Ping();
                if (!connected)
                {
                    badConnections.Add(connection);
                }
            }
            user.ConnectionIds.RemoveAll(badConnections.Contains);
            return user;
        }
    }
}