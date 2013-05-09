using System.Collections.Generic;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Infrastructure.RavenDB.Model
{
    public class RavenUser : EntityBase
    {
        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }

        public List<string> FeedIds { get; set; }
    }
}