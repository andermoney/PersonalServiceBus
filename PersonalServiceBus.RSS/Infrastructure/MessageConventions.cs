using NServiceBus;

namespace PersonalServiceBus.RSS
{
    public class MessageConventions : IWantToRunBeforeConfiguration
    {
        public void Init()
        {
            Configure.Instance
            .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("PersonalServiceBus.RSS.Messages"))
            .DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("PersonalServiceBus.Contract"));
        }
    }
}

