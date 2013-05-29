using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Infrastructure.RavenDB;

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
        public void AddFeedTest()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            var feed = new Feed();
            var response = feedManager.AddFeed(feed);

            //Assert
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
        }

        [Test]
        public void UpdateFeedTest()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            var feed = new Feed();
            var response = feedManager.UpdateFeed(feed);

            //Assert
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
        }

        [Test]
        public void GetFeedsTest_UserIdRequired()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            var user = new User();
            var response = feedManager.GetFeeds(user);

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
            var response = feedManager.GetFeeds(user);

            //Assert
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
        }
    }
}
