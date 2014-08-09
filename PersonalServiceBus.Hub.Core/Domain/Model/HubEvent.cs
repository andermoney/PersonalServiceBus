namespace PersonalServiceBus.Hub.Core.Domain.Model
{
    public class HubEvent
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Host { get; set; }
    }
}