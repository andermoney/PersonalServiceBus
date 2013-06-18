using System;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Infrastructure.RavenDB.Model
{
    public class RavenFeed : EntityBase
    {
        public string Url { get; set; }

        public DateTime FeedRetrieveDate { get; set; }

        public Status Status { get; set; }
    }
}