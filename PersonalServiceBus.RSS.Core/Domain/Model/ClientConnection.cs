using System;

namespace PersonalServiceBus.RSS.Core.Domain.Model
{
    public class ClientConnection : EntityBase
    {
        public string Username { get; set; }

        public string ConnectionId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}