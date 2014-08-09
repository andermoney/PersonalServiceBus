using System.Collections.Generic;

namespace PersonalServiceBus.RSS.Core.Domain.Model
{
    public class Connection
    {
        public string UserId { get; set; }
        public List<string> ConnectionIds { get; set; }
    }
}