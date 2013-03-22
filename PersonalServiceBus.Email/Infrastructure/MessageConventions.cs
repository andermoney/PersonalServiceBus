using NServiceBus;

namespace PersonalServiceBus.Email
{
    public class MessageConventions : IWantToRunBeforeConfiguration
    {
        public void Init()
        {
            Configure.Instance
            .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("PersonalServiceBus.Email.Messages"))
            .DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("PersonalServiceBus.Contract"));
        }
    }
}

