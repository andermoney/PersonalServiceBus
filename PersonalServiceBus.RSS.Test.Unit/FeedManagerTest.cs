using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Ninject;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Infrastructure.RavenDB;
using PersonalServiceBus.RSS.Test.Unit.IoC;

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
            _database = TestRegistry.GetKernel().Get<IDatabase>();
        }

        [Test]
        public void AddUserFeed_UserFeedIsRequired()
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
        public void AddUserFeed_FeedIsRequired()
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
        public void AddUserFeed()
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
        public void UpdateUserFeedTest()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            var unreadCount = new Random().Next(1000);
            const string url = "http://test.url.fake";
            var userFeed = new UserFeed
                {
                    Id = "ravenuserfeed/1",
                    Category = "updated category",
                    Name = "updated name",
                    UnreadCount = unreadCount,
                    Feed = new Feed
                        {
                            Id = "ravenfeed/1",
                            Url = url
                        },
                    RavenUserId = "ravenuser/1"
                };
            var response = feedManager.UpdateUserFeed(userFeed);

            //Assert
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
            var getFeedResponse = feedManager.GetUserFeedByUserIdAndUrl(new User {Id = "ravenuser/1"}, url);
            Assert.IsNotNull(getFeedResponse);
            Assert.AreEqual(ErrorLevel.None, getFeedResponse.Status.ErrorLevel, getFeedResponse.Status.ErrorMessage);
            Assert.IsNotNull(getFeedResponse.Data);
            Assert.IsNotNull(getFeedResponse.Data.Feed);
            Assert.AreEqual(url, getFeedResponse.Data.Feed.Url);
            Assert.AreEqual(unreadCount, getFeedResponse.Data.UnreadCount);
            Assert.AreEqual("updated category", getFeedResponse.Data.Category);
            Assert.AreEqual("updated name", getFeedResponse.Data.Name);
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
            //TODO there has to be a better way to deal with eventual consistency here
            Thread.Sleep(500);
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
        public void GetUserFeedsTest()
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

        [Test]
        public void GetUserFeedByUserIdAndUrl()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            const string ravenUserId = "ravenuser/1";
            var user = new User
                {
                    Id = ravenUserId
                };
            const string url = "http://test.url.fake";
            var response = feedManager.GetUserFeedByUserIdAndUrl(user, url);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(url, response.Data.Feed.Url);
            Assert.AreEqual(ravenUserId, response.Data.RavenUserId);
        }

        [Test]
        public void GetUserFeedUnreadCount_FeedRequired()
        {
            //Arrange
            var feedManager = TestRegistry.GetKernel().Get<IFeedManager>();

            //Act
            var userFeed = new UserFeed
            {
                Id = "UserFeeds/1"
            };
            var response = feedManager.GetUserFeedUnreadCount(userFeed);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(ErrorLevel.Error, response.Status.ErrorLevel);
            Assert.IsTrue(response.Status.ErrorMessage.Contains("Feed cannot be null"), 
                "Error message should contain \"Feed cannot be null\": " + response.Status.ErrorMessage);
        }

        [Test]
        public void GetUserFeedUnreadCount()
        {
            //Arrange
            var feedManager = TestRegistry.GetKernel().Get<IFeedManager>();

            //Act
            var userFeed = new UserFeed
                {
                    Id = "UserFeeds/1",
                    RavenUserId = "ravenuser/1",
                    Feed = new Feed
                        {
                            Id = "ravenfeed/1"
                        }
                };
            var response = feedManager.GetUserFeedUnreadCount(userFeed);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
            Assert.Greater(response.Data, 0);
        }

        [Test]
        public void GetNextFeed()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            var response = feedManager.GetNextFeed();

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
            Assert.IsNotNull(response.Data);
        }

        [Test]
        public void AddFeedItems()
        {
            //Arrange
            var feedManager = TestRegistry.GetKernel().Get<IFeedManager>();

            //Act
            IEnumerable<FeedItem> newFeedItems = new List<FeedItem>
                {
                    new FeedItem
                        {
                            Title = "Test item"
                        }
                };
            var response = feedManager.AddFeedItems(newFeedItems);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
            Assert.IsNotNull(response.Data);
            Assert.Greater(response.Data.Count(), 0);
            Assert.IsNotNull(response.Data.First().Id);
        }

        [Test]
        public void AddUserFeedItems()
        {
            //Arrange
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            IEnumerable<FeedItem> newFeedItems = new List<FeedItem>
                {
                    new FeedItem
                        {
                            Id = "FeedItem/1"
                        }
                };
            var user = new User
                {
                    Id = "RavenUsers/1"
                };
            CollectionResponse<UserFeedItem> response = feedManager.AddUserFeedItems(newFeedItems, user);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
            Assert.IsNotNull(response.Data);
            Assert.Greater(response.Data.Count(), 0);
            Assert.IsNotNull(response.Data.First().FeedItemId);
            Assert.IsTrue(response.Data.First().IsUnread);
        }

        [Test]
        public void GetUserFeedItems()
        {
            //Arrange
            //TODO use servicelocator for this
            IFeedManager feedManager = new FeedManager(_database);

            //Act
            var userFeed = new UserFeed();
            CollectionResponse<UserFeedItem> response = feedManager.GetUserFeedItems(userFeed);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
            Assert.IsNotNull(response.Data);
            Assert.Greater(response.Data.Count(), 0);
            Assert.IsNotNull(response.Data.First().Id);
        }
    }
}
// ReSharper restore InconsistentNaming
