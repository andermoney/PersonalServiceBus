using ServiceStack;

namespace PersonalServiceBus.Hub.Messages
{
    [Route("/subscribe/{Publisher}")]
    public class SubscribeRequest
    {
        public string Publisher { get; set; }
    }
}