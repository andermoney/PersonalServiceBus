using NUnit.Framework;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
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
    }
}