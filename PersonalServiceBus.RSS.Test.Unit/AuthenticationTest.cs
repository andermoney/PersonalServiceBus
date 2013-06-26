using NUnit.Framework;
using Ninject;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Test.Unit.IoC;

namespace PersonalServiceBus.RSS.Test.Unit
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class AuthenticationTest
    {
        [Test]
        public void Register_PasswordRequired()
        {
            //Arrange
            var authentication = TestRegistry.GetKernel().Get<IAuthentication>();

            //Act
            var user = new User
                {
                    Username = "testuser2",
                    Email = "fakeemail@jake-anderson.com"
                };
            var status = authentication.Register(user);

            //Assert
            Assert.IsNotNull(status);
            Assert.AreEqual(ErrorLevel.Error, status.ErrorLevel, "Password should be required");
            Assert.IsTrue(status.ErrorMessage.Contains("Password is required"), "Error message was: " + status.ErrorMessage);
        }

        [Test]
        public void Register_EmailRequired()
        {
            //Arrange
            var authentication = TestRegistry.GetKernel().Get<IAuthentication>();

            //Act
            var user = new User
                {
                    Username = "testuser2",
                    Password = "Abc123"
                };
            var status = authentication.Register(user);

            //Assert
            Assert.IsNotNull(status);
            Assert.AreEqual(ErrorLevel.Error, status.ErrorLevel, "Email should be required");
            Assert.IsTrue(status.ErrorMessage.Contains("Email is required"), "Error message was: " + status.ErrorMessage);
        }

        [Test]
        public void Register_UsernameRequired()
        {
            //Arrange
            var authentication = TestRegistry.GetKernel().Get<IAuthentication>();

            //Act
            var user = new User
            {
                Password = "Abc123",
                Email = "fakeemail@jake-anderson.com"
            };
            var status = authentication.Register(user);

            //Assert
            Assert.IsNotNull(status);
            Assert.AreEqual(ErrorLevel.Error, status.ErrorLevel, "Username should be required");
            Assert.IsTrue(status.ErrorMessage.Contains("Username is required"), "Error message was: " + status.ErrorMessage);
        }

        [Test]
        public void Register_UserExists()
        {
            //Arrange
            var authentication = TestRegistry.GetKernel().Get<IAuthentication>();

            //Act
            var user = new User
            {
                Username = "testuser",
                Password = "Abc123",
                Email = "fakeemail@jake-anderson.com"
            };
            var status = authentication.Register(user);

            //Assert
            Assert.IsNotNull(status);
            Assert.AreEqual(ErrorLevel.Error, status.ErrorLevel, "Username should throw error for existing");
            Assert.IsTrue(status.ErrorMessage.Contains("User name already exists"), "Error message was: " + status.ErrorMessage);
        }

        [Test]
        public void Register()
        {
            //Arrange
            var authentication = TestRegistry.GetKernel().Get<IAuthentication>();

            //Act
            var user = new User
                {
                    Username = "testuser2",
                    Password = "Abc123",
                    Email = "fakeemail@jake-anderson.com"
                };
            var status = authentication.Register(user);

            //Assert
            Assert.IsNotNull(status);
            Assert.AreEqual(ErrorLevel.None, status.ErrorLevel, status.ErrorMessage);
        }

        [Test]
        public void ValidateUser_InvalidPassword()
        {
            //Arrange
            var authentication = TestRegistry.GetKernel().Get<IAuthentication>();

            //Act
            var user = new User
            {
                Username = "testuser",
                Password = "Abc1234",
                Email = "fakeemail@jake-anderson.com"
            };
            var response = authentication.ValidateUser(user);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(ErrorLevel.Error, response.Status.ErrorLevel, "Password should be invalid");
            Assert.IsTrue(response.Status.ErrorMessage.Contains("Username or password is not valid"), "Error message was: " + response.Status.ErrorMessage);
        }

        [Test]
        public void ValidateUser()
        {
            //Arrange
            var authentication = TestRegistry.GetKernel().Get<IAuthentication>();

            //Act
            var user = new User
            {
                Username = "testuser",
                Password = "Abc123",
                Email = "fakeemail@jake-anderson.com"
            };
            var response = authentication.ValidateUser(user);

            //Assert
            Assert.IsNotNull(response);
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
        }

        [Test]
        public void ChangePassword()
        {
            Assert.Fail("Not written yet");
        }

        [Test]
        public void GetUserByUsername()
        {
            Assert.Fail("Not written yet");
        }

        [Test]
        public void GetUserByUserId()
        {
            //Arrange
            var authentication = TestRegistry.GetKernel().Get<IAuthentication>();
            
            //Act
            SingleResponse<User> response = authentication.GetUserByUserId(new User
                {
                    Id = "RavenUser/1"
                });

            //Assert
            Assert.AreEqual(ErrorLevel.None, response.Status.ErrorLevel, response.Status.ErrorMessage);
            Assert.IsNotNull(response.Data);
        }

        [Test]
        public void UpdateUser()
        {
            Assert.Fail("Not written yet");
        }

        [Test]
        public void AddConnection()
        {
            Assert.Fail("Not written yet");
        }

        [Test]
        public void RemoveConnection()
        {
            Assert.Fail("Not written yet");
        }

        [Test]
        public void UpdateConnection()
        {
            Assert.Fail("Not written yet");
        }
        // ReSharper restore InconsistentNaming
    }
}