using System;

namespace PersonalServiceBus.RSS.Messages.Feeds
{
    public class AddFeedResponse
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public Exception ErrorException { get; set; }
    }
}