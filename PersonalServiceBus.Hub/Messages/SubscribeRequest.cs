using ServiceStack;

namespace PersonalServiceBus.Hub.Messages
{
    [Route("/subscribe/{Publisher}", "POST")]
    public class SubscribeRequest
    {
        public string Publisher { get; set; }
        public string Host { get; set; }
    }
}