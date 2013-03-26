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
                .DefineEndpointName("personalservicebus.rss")
                .IsTransactional(false)
                .PurgeOnStartup(false)
                .RunTimeoutManagerWithInMemoryPersistence()
                .UnicastBus()
                .ImpersonateSender(false)
                .CreateBus()
                .Start();
        }
    }
}