using NUnit.Framework;
using Ninject;
using PersonalServiceBus.RSS.Core.Domain.Interface;
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
            IDatabase database = TestRegistry.GetKernel().Get<IDatabase>();

            //Act
            string id = database.Store(new RavenFeed());

            //Assert
            Assert.IsNotNull(id);
            Assert.IsTrue(id.StartsWith("RavenFeeds"), string.Format("id does not start with RavenFeeds: [{0}]", id));
        }
    }
    // ReSharper restore InconsistentNaming
}