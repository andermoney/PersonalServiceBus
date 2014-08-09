using NUnit.Framework;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Core.Helper;
using PersonalServiceBus.RSS.Infrastructure.RSS;

namespace PersonalServiceBus.RSS.Test.Unit
{
    [TestFixture]
    public class RssManagerTest
    {
        [Test]
        public void GetFeedItemsTest()
        {
            //Arrange
            IRssManager rssManager = new RssManager();

            //Act
            var feedItems = rssManager.GetFeedItems(new Feed
                {
                    Url = "http://feeds.feedburner.com/stereogum/cBYa"
                });

            //Assert
            Assert.IsNotNull(feedItems);
            Assert.AreEqual(ErrorLevel.None, feedItems.Status.ErrorLevel, feedItems.Status.ErrorMessage);
            Assert.IsNotNull(feedItems.Data);
            Assert.IsNotEmpty(feedItems.Data);
        }

        [Test]
        public void LookupUserFeed()
        {
            //Arrange
            IRssManager rssManager = new RssManager();

            //Act
            const string url = "http://ericlippert.com/feed/";
            var userFeed = new UserFeed
            {
                Feed = new Feed
                {
                    Url = url
                }
            };
            SingleResponse<UserFeed> response = rssManager.LookupUserFeed(userFeed);

            //Assert
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
            Assert.IsNotNullOrEmpty(response.Data.Name);
        }

        [Test]
        public void LookupUserFeed_CatchUriException()
        {
            //Arrange
            IRssManager rssManager = new RssManager();

            //Act
            var userFeed = new UserFeed
            {
                Feed = new Feed
                {
                    Url = ""
                }
            };
            var response = rssManager.LookupUserFeed(userFeed);

            //Assert
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
        }

        [Test]
        public void LookupUserFeed_NotSupportedUrl()
        {
            //Arrange
            IRssManager rssManager = new RssManager();

            //Act
            var userFeed = new UserFeed
            {
                Feed = new Feed
                {
                    Url = "magnet:?xt=urn:btih:d784a1cdfe1f3b75de71f59be0a75bf68a00806c&dn=%5BMetal%5"
                }
            };
            var response = rssManager.LookupUserFeed(userFeed);

            //Assert
            Assert.AreEqual(ErrorLevel.Error, response.Status.ErrorLevel, "Should have gotten an error: " + response.Status.ErrorMessage);
        }
    }
}