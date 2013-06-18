using System.Linq;
using NUnit.Framework;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Infrastructure.RavenDB;
using PersonalServiceBus.RSS.Test.Unit.Helper;

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
            _database = DatabaseBuilder.BuildTestDatabase();
        }

        [Test]
        public void AddFeed_UserFeedIsRequired()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            var response = feedManager.AddUserFeed(null);

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
            var response = feedManager.AddUserFeed(userFeed);

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
            const string url = "http://test.url.fake";
            var userFeed = new UserFeed
                {
                    Feed = new Feed
                        {
                            Url = url
                        }
                };
            var response = feedManager.AddUserFeed(userFeed);

            //Assert
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
            var getFeedResponse = feedManager.GetUserFeedsByUrl(url);
            Assert.AreEqual(ErrorLevel.None, getFeedResponse.Status.ErrorLevel, getFeedResponse.Status.ErrorMessage);
            Assert.IsNotNull(getFeedResponse.Data);
            Assert.IsNotEmpty(getFeedResponse.Data);
            var feedResponse = getFeedResponse.Data.First();
            Assert.IsNotNull(feedResponse);
            Assert.AreEqual(url, feedResponse.Feed.Url);
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
        public void GetUserFeeds()
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
            Assert.IsNotNull(response.Data);
            Assert.IsNotEmpty(response.Data);
            var firstFeed = response.Data.First();
            Assert.IsNotNull(firstFeed);
            Assert.IsNotNull(firstFeed.Name);
        }

        [Test]
        public void GetUserFeeds_AfterAdd()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            const string url = "http://new.feed.url";
            var userFeed = new UserFeed
            {
                Feed = new Feed
                {
                    Url = url
                },
                RavenUserId = "ravenuser/2"
            };
            var response = feedManager.AddUserFeed(userFeed);

            //Assert
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
            var getFeedsResponse = feedManager.GetUserFeeds(new User
                {
                    Id = "ravenuser/2"
                });
            Assert.AreEqual(ErrorLevel.None, getFeedsResponse.Status.ErrorLevel, getFeedsResponse.Status.ErrorMessage);
            Assert.IsNotNull(getFeedsResponse.Data);
            Assert.IsNotEmpty(getFeedsResponse.Data);
            var newFeed = getFeedsResponse.Data.FirstOrDefault(f => f.Feed.Url == url);
            Assert.IsNotNull(newFeed);
            Assert.AreEqual(newFeed.Feed.Url, url);
            Assert.AreEqual("ravenuser/2", newFeed.RavenUserId);
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
