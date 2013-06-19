using NUnit.Framework;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Infrastructure.RavenDB;

namespace PersonalServiceBus.RSS.Test.Unit
{
    [TestFixture]
    public class AuthenticationTest
    {
        [Test]
        public void GetUserByUserId()
        {
            //Arrange
            //IAuthentication authentication = new RavenDBAuthentication(_database, _cryptography, _configuration);
            
            //Act
            //SingleResponse<User> user = authentication.GetUserByUserId("RavenUser/1");

            //Assert
            //Assert.IsNotNull(user);
            Assert.Fail("Need to figure out dependency injection for unit tests");
        }
    }
}