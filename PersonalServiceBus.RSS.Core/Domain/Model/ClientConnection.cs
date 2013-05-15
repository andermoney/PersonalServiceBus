using System;
using System.Collections.Generic;

namespace PersonalServiceBus.RSS.Core.Domain.Model
{
    public class ClientConnection : EntityBase
    {
        public string Username { get; set; }

        public List<string> ConnectionIds { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}