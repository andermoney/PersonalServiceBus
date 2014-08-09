using Ninject;
using NUnit.Framework;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Infrastructure.RavenDB;
using PersonalServiceBus.RSS.Test.Unit.IoC;

namespace PersonalServiceBus.RSS.Test.Unit
{
    [TestFixture]
    public class ConnectionManagerTest
    {
        private IDatabase _database;

        [SetUp]
        public void SetUp()
        {
            _database = TestRegistry.GetKernel().Get<IDatabase>();
        }

        [Test]
        public void AddConnectionTest()
        {
            //Arrange
            IConnectionManager connectionManager = new ConnectionManager(_database);

            //Act
            var connection = new Connection();
            var response = connectionManager.AddConnection(connection);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Data);
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
        }

        [Test]
        public void GetAllConnectionsTest()
        {
            //Arrange
            IConnectionManager connectionManager = new ConnectionManager(_database);

            //Act
            var response = connectionManager.GetAllConnections();

            //Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Data);
            Assert.IsNotEmpty(response.Data);
        }
    }
}