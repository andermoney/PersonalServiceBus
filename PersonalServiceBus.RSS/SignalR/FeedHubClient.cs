using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Ninject;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Infrastructure;

namespace PersonalServiceBus.RSS.SignalR
{
    public class FeedHubClient
    {
        private IAuthentication _authentication;

        public FeedHubClient()
        {
            _authentication = NinjectRegistry.GetKernel().Get<IAuthentication>();
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get { return GlobalHost.ConnectionManager.GetHubContext<FeedHub>().Clients; }
        }

        public void UpdateFeedUnreadCount(string username, UserFeed userFeed)
        {
            Clients.Group(username).UpdateFeedUnreadCount(userFeed);
        }

        public void CheckClientConnections()
        {
            var response = _authentication.GetAllConnections();
            foreach (var user in response.Data)
            {
                foreach (var connection in user.ConnectionIds)
                {
                    var connected = Clients.Client(connection).Ping();
                    if (!connected)
                    {
                        _authentication.RemoveConnection(connection, new User
                        {
                            Id = user.UserId
                        });
                    }
                }
            }
        }
    }
}