using System;
using System.Collections.Generic;

namespace PersonalServiceBus.RSS.Core.Domain.Model
{
    public class User : EntityBase
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public List<string> FeedIds { get; set; }

        public List<string> ConnectionIds { get; set; }
    
        public List<string> UnreadFeedItemIds { get; set; }

        public DateTime LastConnectedDate { get; set; }
    }
}