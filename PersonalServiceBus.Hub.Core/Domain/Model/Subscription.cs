namespace PersonalServiceBus.Hub.Core.Domain.Model
{
    public class Subscription
    {
        public string Publisher { get; set; }
        public string Host { get; set; }
    }
}