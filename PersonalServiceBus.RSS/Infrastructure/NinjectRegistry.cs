using Ninject;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Infrastructure.RavenDB;
using PersonalServiceBus.RSS.Infrastructure.RSS;
using PersonalServiceBus.RSS.SignalR;

namespace PersonalServiceBus.RSS.Infrastructure
{
    public class NinjectRegistry
    {
        private static IKernel _kernel;

        public static IKernel GetKernel()
        {
            return _kernel ?? (_kernel = Register());
        }

        public static void Register(IKernel kernel)
        {
            kernel.Bind<IFeedManager>().To<FeedManager>().InSingletonScope();
            kernel.Bind<IRssManager>().To<RssManager>().InSingletonScope();
            kernel.Bind<IDatabase>().To<RavenDatabase>().InSingletonScope();
            kernel.Bind<IConfiguration>().To<WebConfiguration>().InSingletonScope();
            kernel.Bind<FeedHubClient>().To<FeedHubClient>().InSingletonScope();
            kernel.Bind<IAuthentication>().To<RavenDBAuthentication>().InSingletonScope();
            kernel.Bind<ICryptography>().To<Cryptography.Cryptography>().InTransientScope();            
        }

        private static IKernel Register()
        {
            var standardKernel = new StandardKernel();
            Register(standardKernel);
            return standardKernel;
        }
    }
}