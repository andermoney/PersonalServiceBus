using Moq;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Infrastructure.RavenDB;
using PersonalServiceBus.RSS.Infrastructure.RavenDB.Model;

namespace PersonalServiceBus.RSS.Test.Unit.Helper
{
    public class DatabaseBuilder
    {
        public static RavenMemoryDatabase BuildEmptyDatabase()
        {
            var configuration = new Mock<IConfiguration>();
            configuration.SetupGet(c => c.RavenDBUrl)
                         .Returns("http://localhost:8080");

            return new RavenMemoryDatabase(configuration.Object);
        }

        public static RavenMemoryDatabase BuildTestDatabase()
        {
            var database = BuildEmptyDatabase();
            database.Store(new RavenUser
                {
                    Username = "testuser"
                });
            return database;
        }
    }
}