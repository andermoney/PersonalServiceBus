using NServiceBus;

namespace PersonalServiceBus.RSS.Infrastructure
{
    public static class WebGlobalInitialization
    {
        public static IBus InitializeNServiceBus()
        {
            return Configure.With()
                .Log4Net()
                .DefaultBuilder()
                .ForMvc()
                .XmlSerializer()
                .MsmqTransport()
                .IsTransactional(false)
                .PurgeOnStartup(false)
                .UnicastBus()
                .ImpersonateSender(false)
                .CreateBus()
                .Start();
        }
    }
}