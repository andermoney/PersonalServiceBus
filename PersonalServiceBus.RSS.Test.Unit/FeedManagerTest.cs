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
        [Test]
        public void AddFeedTest()
        {
            //Arrange
            var configuration = new Mock<IConfiguration>();
            IDatabase database = new RavenDatabase(configuration.Object);
            IFeedManager feedManager = new FeedManager(database);

            //Act
            Feed feed = new Feed();
            var response = feedManager.AddFeed(feed);

            //Assert
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel);
        }
    }
}
