using Moq;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Infrastructure.Cryptography;
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
            var cryptography = new Cryptography();
            const string ravenUserId = "ravenuser/1";
            var testUser = new RavenUser
                {
                    Id = ravenUserId,
                    Username = "testuser",
                    PasswordHash = cryptography.CreateHash("Abc123")
                };
            database.Store(testUser);
            const string feedId = "ravenfeed/1";
            var testFeed = new RavenFeed
                {
                    Id = feedId, 
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
            const string feedItemId = "FeedItems/1";
            database.Store(new FeedItem
                {
                    FeedId = feedId
                });
            database.Store(new UserFeedItem
                {
                    FeedId = feedId,
                    RavenUserId = ravenUserId,
                    FeedItemId = feedItemId,
                    IsUnread = true
                });
            database.Store(new Connection());
            return database;
        }
    }
}