using System.Collections.Generic;

namespace PersonalServiceBus.RSS.Core.Domain.Model
{
    public class FeedCategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<UserFeed> Feeds { get; set; } 
    }
}