using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using PersonalServiceBus.RSS.Models;

namespace PersonalServiceBus.RSS.SignalR
{
    [HubName("feedHub")]
    public class FeedHub : Hub
    {
        public IEnumerable<Category> GetFeedCategories()
        {
            return new List<Category>
                {
                    new Category
                        {
                            CategoryId = 1,
                            CategoryName = "Webcomics"
                        }
                };
        }
    }
}