using System;
using System.Linq;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Infrastructure.RavenDB.Model;

namespace PersonalServiceBus.RSS.Infrastructure.RavenDB
{
    public class RavenDBAuthentication : IAuthentication
    {
        private readonly IDatabase _database;
        private readonly ICryptography _cryptography;

        public RavenDBAuthentication(IDatabase database,
            ICryptography cryptography)
        {
            _database = database;
            _cryptography = cryptography;
        }

        public Status Register(User user)
        {
            try
            {
                var storageUser = new RavenUser
                    {
                        Username = user.Username,
                        PasswordHash = _cryptography.CreateHash(user.Password),
                        Email = user.Email
                    };
                _database.Store(storageUser);
                return new Status
                    {
                        ErrorLevel = ErrorLevel.None
                    };
            }
            catch (Exception ex)
            {
                return new Status
                    {
                        ErrorLevel = ErrorLevel.Critical,
                        ErrorMessage = "Fatal error registering user",
                        ErrorException = ex
                    };
            }
        }

        public SingleResponse<bool> ValidateUser(User user)
        {
            try
            {
                var storageUser = GetUserByUsername(user);
                var valid = storageUser != null && _cryptography.MatchHash(storageUser.PasswordHash, user.Password); 
                return new SingleResponse<bool>
                    {
                        Data = valid,
                        Status = new Status
                            {
                                ErrorLevel = valid ? ErrorLevel.None : ErrorLevel.Error,
                                ErrorMessage = valid ? "" : "Username or password is not valid"
                            }
                    };
            }
            catch (Exception ex)
            {
                return new SingleResponse<bool>
                    {
                        Data = false,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Critical,
                                ErrorMessage = "Fatal error logging in",
                                ErrorException = ex
                            }
                    };
            }
        }

        public SingleResponse<bool> ChangePassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                var user = new User(username, oldPassword);
                var storageUser = GetUserByUsername(user);
                var valid = storageUser != null && _cryptography.MatchHash(storageUser.PasswordHash, user.Password); 
                if (!valid)
                {
                    return new SingleResponse<bool>
                    {
                        Data = false,
                        Status = new Status
                        {
                            ErrorLevel = ErrorLevel.Error,
                            ErrorMessage = "Username or password is not valid"
                        }
                    };
                }
                storageUser.PasswordHash = _cryptography.CreateHash(newPassword);
                _database.Store(storageUser);
                return new SingleResponse<bool>
                    {
                        Data = true,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.None
                            }
                    };
            }
            catch (Exception ex)
            {
                return new SingleResponse<bool>
                {
                    Data = false,
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.Critical,
                        ErrorMessage = "Fatal error changing password",
                        ErrorException = ex
                    }
                };                
            }
        }

        private RavenUser GetUserByUsername(User user)
        {
            var storageUser = _database.Query<RavenUser>()
                                       .FirstOrDefault(u => u.Username == user.Username);
            return storageUser;
        }

        //private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        //{
        //    // See http://go.microsoft.com/fwlink/?LinkID=177550 for
        //    // a full list of status codes.
        //    switch (createStatus)
        //    {
        //        case MembershipCreateStatus.DuplicateUserName:
        //            return "User name already exists. Please enter a different user name.";

        //        case MembershipCreateStatus.DuplicateEmail:
        //            return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

        //        case MembershipCreateStatus.InvalidPassword:
        //            return "The password provided is invalid. Please enter a valid password value.";

        //        case MembershipCreateStatus.InvalidEmail:
        //            return "The e-mail address provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.InvalidAnswer:
        //            return "The password retrieval answer provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.InvalidQuestion:
        //            return "The password retrieval question provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.InvalidUserName:
        //            return "The user name provided is invalid. Please check the value and try again.";

        //        case MembershipCreateStatus.ProviderError:
        //            return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        //        case MembershipCreateStatus.UserRejected:
        //            return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        //        default:
        //            return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
        //    }
        //}
    }
}