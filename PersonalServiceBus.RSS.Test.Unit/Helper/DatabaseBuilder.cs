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
            var testUser = new RavenUser
                {
                    Id = "ravenuser/1",
                    Username = "testuser"
                };
            database.Store(testUser);
            var testFeed = new RavenFeed
                {
                    Id = "ravenfeed/1", 
                    Url = "http://test.url.fake"
                };
            database.Store(testFeed);
            database.Store(new RavenUserFeed
                {
                    Category = "testuser's test feeds",
                    RavenFeedId = testFeed.Id,
                    Id = "ravenuserfeed/1",
                    Name = "testuser's test feed",
                    RavenUserId = testUser.Id
                });
            return database;
        }
    }
}