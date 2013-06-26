using Moq;
using Ninject;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Infrastructure.Cryptography;
using PersonalServiceBus.RSS.Infrastructure.RSS;
using PersonalServiceBus.RSS.Infrastructure.RavenDB;
using PersonalServiceBus.RSS.Test.Unit.Helper;

namespace PersonalServiceBus.RSS.Test.Unit.IoC
{
    public static class TestRegistry
    {
        private static IKernel _kernel;

        private static IKernel Register()
        {
            IKernel kernel = new StandardKernel();
            kernel.Bind<IFeedManager>().To<FeedManager>().InSingletonScope();
            kernel.Bind<IRssManager>().To<RssManager>().InSingletonScope();
            kernel.Bind<IDatabase>().ToConstant(DatabaseBuilder.BuildTestDatabase()).InSingletonScope();
            
            var configuration = new Mock<IConfiguration>();
            configuration.SetupGet(c => c.RavenDBUrl)
                         .Returns("http://localhost:8080");
            configuration.SetupGet(c => c.PasswordRegex)
                         .Returns(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$");

            kernel.Bind<IConfiguration>().ToConstant(configuration.Object).InSingletonScope();
            kernel.Bind<IAuthentication>().To<RavenDBAuthentication>().InSingletonScope();
            kernel.Bind<ICryptography>().To<Cryptography>();
            return kernel;
        }

        public static IKernel GetKernel()
        {
            return _kernel ?? (_kernel = Register());
        }
    }
}