using System;

namespace PersonalServiceBus.RSS.Core.Domain.Model
{
    public class Feed
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        
        public DateTime FeedRetrieveDate { get; set; }
    }
}