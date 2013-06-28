using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ninject;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Infrastructure.RavenDB.Model;
using PersonalServiceBus.RSS.Test.Unit.IoC;

namespace PersonalServiceBus.RSS.Test.Unit
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class DatabaseTest
    {
        [Test]
        public void Store_ReturnsId()
        {
            //Arrange
            var database = TestRegistry.GetKernel().Get<IDatabase>();

            //Act
            string id = database.Store(new RavenFeed());

            //Assert
            Assert.IsNotNull(id);
            Assert.IsTrue(id.StartsWith("RavenFeeds"), string.Format("id does not start with RavenFeeds: [{0}]", id));
        }

        [Test]
        public void StoreCollection_ReturnsIds()
        {
            //Arrange
            var database = TestRegistry.GetKernel().Get<IDatabase>();

            //Act
            var items = database.StoreCollection(new List<FeedItem>
                {
                    new FeedItem()
                }).ToList();

            //Assert
            Assert.IsNotNull(items);
            Assert.IsNotEmpty(items);
            var id = items.First().Id;
            Assert.IsTrue(id.StartsWith("FeedItems"), string.Format("id does not start with FeedItems: [{0}]", id));
        }
    }
    // ReSharper restore InconsistentNaming
}