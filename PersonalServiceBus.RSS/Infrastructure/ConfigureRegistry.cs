using NServiceBus;
using PersonalServiceBus.RSS.Infrastructure.RSS;
using PersonalServiceBus.RSS.Infrastructure.RavenDB;
using PersonalServiceBus.RSS.SignalR;

namespace PersonalServiceBus.RSS.Infrastructure
{
    public static class ConfigureRegistry
    {
        public static Configure WithRegistry(this Configure configure)
        {
            configure.Configurer.ConfigureComponent<FeedManager>(DependencyLifecycle.SingleInstance);
            configure.Configurer.ConfigureComponent<RssManager>(DependencyLifecycle.SingleInstance);
            configure.Configurer.ConfigureComponent<RavenDatabase>(DependencyLifecycle.SingleInstance);
            configure.Configurer.ConfigureComponent<WebConfiguration>(DependencyLifecycle.SingleInstance);
            configure.Configurer.ConfigureComponent<FeedHubClient>(DependencyLifecycle.SingleInstance);
            return configure;
        }
    }
}