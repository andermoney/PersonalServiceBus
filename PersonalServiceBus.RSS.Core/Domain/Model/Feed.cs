using System;
using System.Collections.Generic;

namespace PersonalServiceBus.RSS.Core.Domain.Model
{
    public class Feed : EntityBase
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        
        public DateTime FeedRetrieveDate { get; set; }
        public int UnreadCount { get; set; }
        public Status Status { get; set; }

        public List<string> UserIds { get; set; }
    }
}