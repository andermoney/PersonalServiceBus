using Moq;
using NUnit.Framework;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Infrastructure.RavenDB;
// ReSharper disable InconsistentNaming

namespace PersonalServiceBus.RSS.Test.Unit
{
    [TestFixture]
    public class FeedManagerTest
    {
        private IDatabase _database;

        [SetUp]
        public void SetUp()
        {
            var configuration = new Mock<IConfiguration>();
            configuration.SetupGet(c => c.RavenDBUrl)
                .Returns("http://localhost:8080");

            _database = new RavenMemoryDatabase(configuration.Object);
        }

        [Test]
        public void AddFeed_UserFeedIsRequired()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            var response = feedManager.AddFeed(null);

            //Assert
            Assert.AreEqual(ErrorLevel.Error, response.Status.ErrorLevel);
            Assert.AreEqual("userFeed is required", response.Status.ErrorMessage);
        }

        [Test]
        public void AddFeed_FeedIsRequired()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            var userFeed = new UserFeed();
            var response = feedManager.AddFeed(userFeed);

            //Assert
            Assert.AreEqual(ErrorLevel.Error, response.Status.ErrorLevel);
            Assert.AreEqual("userFeed.Feed is required", response.Status.ErrorMessage);
        }

        [Test]
        public void AddFeed()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            var userFeed = new UserFeed
                {
                    Feed = new Feed
                        {
                            Id = "feed/1"
                        }
                };
            var response = feedManager.AddFeed(userFeed);

            //Assert
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
        }

        [Test]
        public void UpdateFeedTest()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            var userFeed = new UserFeed();
            var response = feedManager.UpdateFeed(userFeed);

            //Assert
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
        }

        [Test]
        public void GetUserFeeds_UserIdRequired()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            var user = new User();
            var response = feedManager.GetUserFeeds(user);

            //Assert
            Assert.AreEqual(ErrorLevel.Error, response.Status.ErrorLevel);
            Assert.AreEqual("User Id is required", response.Status.ErrorMessage);
        }

        [Test]
        public void GetFeedsTest()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            var user = new User
                {
                    Id = "ravenuser/1"
                };
            var response = feedManager.GetUserFeeds(user);

            //Assert
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
        }
    }
}
// ReSharper restore InconsistentNaming
