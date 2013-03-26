using NServiceBus;
using PersonalServiceBus.RSS.Infrastructure.RavenDB;

namespace PersonalServiceBus.RSS.Infrastructure
{
    public static class ConfigureRegistry
    {
        public static Configure WithRegistry(this Configure configure)
        {
            configure.Configurer.ConfigureComponent<FeedManager>(DependencyLifecycle.SingleInstance);
            configure.Configurer.ConfigureComponent<RavenDatabase>(DependencyLifecycle.SingleInstance);
            configure.Configurer.ConfigureComponent<WebConfiguration>(DependencyLifecycle.SingleInstance);
            return configure;
        }
    }
}