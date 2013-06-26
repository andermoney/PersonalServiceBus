using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Core.Helper;
using PersonalServiceBus.RSS.Infrastructure.RavenDB.Model;
using IConfiguration = PersonalServiceBus.RSS.Core.Domain.Interface.IConfiguration;

namespace PersonalServiceBus.RSS.Infrastructure.RavenDB
{
    public class RavenDBAuthentication : IAuthentication
    {
        private readonly IDatabase _database;
        private readonly ICryptography _cryptography;
        private readonly IConfiguration _configuration;

        public RavenDBAuthentication(IDatabase database,
            ICryptography cryptography,
            IConfiguration configuration)
        {
            _database = database;
            _cryptography = cryptography;
            _configuration = configuration;

            Mapper.CreateMap<RavenUser, User>();
        }

        //TODO switch to response object
        public Status Register(User user)
        {
            try
            {
                if (user == null)
                {
                    return new Status
                        {
                            ErrorLevel = ErrorLevel.Error,
                            ErrorMessage = "User cannot be null"
                        };
                }
                if (string.IsNullOrEmpty(user.Username))
                {
                    return new Status
                        {
                            ErrorLevel = ErrorLevel.Error,
                            ErrorMessage = "Username is required"
                        };
                }
                if (string.IsNullOrEmpty(user.Password))
                {
                    return new Status
                        {
                            ErrorLevel = ErrorLevel.Error,
                            ErrorMessage = "Password is required"
                        };
                }
                if (string.IsNullOrEmpty(user.Email))
                {
                    return new Status
                        {
                            ErrorLevel = ErrorLevel.Error,
                            ErrorMessage = "Email is required"
                        };
                }
                var storageUser = new RavenUser
                    {
                        Username = user.Username,
                        PasswordHash = _cryptography.CreateHash(user.Password),
                        Email = user.Email
                    };

                Status status = ValidateNewUser(user);
                if (status.ErrorLevel > ErrorLevel.None)
                {
                    return status;
                }

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
                        ErrorMessage = string.Format("Fatal error registering user: {0}", ex),
                        ErrorException = ex
                    };
            }
        }

        public SingleResponse<bool> ValidateUser(User user)
        {
            try
            {
                var storageUser = GetRavenUserByUsername(user);
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
                var user = new User
                { 
                    Username = username, 
                    Password = oldPassword
                };
                var storageUser = GetRavenUserByUsername(user);
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

                user.Password = newPassword;
                if (!PasswordValid(user))
                {
                    return new SingleResponse<bool>
                    {
                        Data = false,
                        Status = new Status
                        {
                            ErrorLevel = ErrorLevel.Error,
                            ErrorMessage = "The password provided is invalid. " + _configuration.PasswordMessage
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

        public SingleResponse<User> GetUserByUsername(User user)
        {
            try
            {
                var ravenUser = GetRavenUserByUsername(user);
                if (ravenUser == null)
                {
                    return new SingleResponse<User>
                        {
                            Data = new User(),
                            Status = new Status
                                {
                                    ErrorLevel = ErrorLevel.Error,
                                    ErrorMessage = string.Format("User \"{0}\" not found", user.Username)
                                }
                        };
                }

                return new SingleResponse<User>
                    {
                        Data = Mapper.Map<User>(ravenUser),
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.None
                            }
                    };
            }
            catch (Exception ex)
            {
                return new SingleResponse<User>
                    {
                        Data = user,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Critical,
                                ErrorMessage = string.Format("Fatal error retrieving user {0}: {1}", user.Username, ex)
                            }
                    };
            }
        }

        public SingleResponse<User> GetUserByUserId(User user)
        {
            try
            {
                var ravenUser = _database.Load<RavenUser>(user.Id);
                if (ravenUser == null)
                {
                    return ResponseBuilder.BuildSingleResponse<User>(ErrorLevel.Error,
                                                                     string.Format("User \"{0}\" not found", user.Username));
                }

                return ResponseBuilder.BuildSingleResponse(Mapper.Map<User>(ravenUser), ErrorLevel.None);
            }
            catch (Exception ex)
            {
                return ResponseBuilder.BuildSingleResponse(user, ErrorLevel.Critical, 
                    string.Format("Fatal error retrieving user {0}: {1}", user.Username, ex), ex);
            }
        }

        public SingleResponse<User> UpdateUser(User user)
        {
            try
            {
                var ravenUser = GetRavenUser(user.Id);

                _database.Store(ravenUser);

                return ResponseBuilder.BuildSingleResponse(user, ErrorLevel.None);
            }
            catch (Exception ex)
            {
                return ResponseBuilder.BuildSingleResponse(user, ErrorLevel.Critical, 
                    string.Format("Fatal error updating user {0}: {1}", user.Username, ex), ex);
            }
        }

        public CollectionResponse<string> GetConnectionByUsername(User user)
        {
            try
            {
                var connectionIds = _database.Query<User>()
                                          .Where(u => u.Username == user.Username)
                                          .Select(u => u.ConnectionIds)
                                          .FirstOrDefault();
                return ResponseBuilder.BuildCollectionResponse(connectionIds, ErrorLevel.None);
            }
            catch (Exception ex)
            {
                return ResponseBuilder.BuildCollectionResponse<string>(ErrorLevel.Critical, 
                    string.Format("Fatal error getting client connections: {0}", ex), ex);
            }
        }

        public SingleResponse<User> AddConnection(string connectionId, User user)
        {
            try
            {
                var existingUser = GetRavenUserByUsername(user);

                if (existingUser.ConnectionIds == null)
                {
                    existingUser.ConnectionIds = new List<string>();
                }

                if (existingUser.ConnectionIds.Contains(connectionId))
                {
                    return new SingleResponse<User>
                        {
                            Data = user,
                            Status = new Status
                                {
                                    ErrorLevel = ErrorLevel.Error,
                                    ErrorMessage = "Connection ID already exists"
                                }
                        };
                }

                existingUser.ConnectionIds.Add(connectionId);
                existingUser.LastConnectedDate = DateTime.Now;
                _database.Store(existingUser);
                return new SingleResponse<User>
                {
                    Data = Mapper.Map<User>(existingUser),
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.None
                    }
                };
            }
            catch (Exception ex)
            {
                return new SingleResponse<User>
                {
                    Data = user,
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.Critical,
                        ErrorMessage = string.Format("Fatal error adding connection: {0}", "ARG0"),
                        ErrorException = ex
                    }
                };
            }
        }

        public SingleResponse<User> RemoveConnection(string connectionId, User user)
        {
            try
            {
                var existingUser = GetRavenUserByUsername(user);

                if (existingUser.ConnectionIds == null)
                {
                    existingUser.ConnectionIds = new List<string>();
                }

                if (existingUser.ConnectionIds.Contains(connectionId))
                {
                    existingUser.ConnectionIds.Remove(connectionId);
                }

                existingUser.LastConnectedDate = DateTime.Now;
                _database.Store(existingUser);
                return new SingleResponse<User>
                {
                    Data = Mapper.Map<User>(existingUser),
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.None
                    }
                };
            }
            catch (Exception ex)
            {
                return new SingleResponse<User>
                {
                    Data = user,
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.Critical,
                        ErrorMessage = string.Format("Fatal error removing connection: {0}", "ARG0"),
                        ErrorException = ex
                    }
                };
            }
        }

        public SingleResponse<User> UpdateConnection(string connectionId, User user)
        {
            try
            {
                var existingUser = GetRavenUserByUsername(user);

                if (existingUser.ConnectionIds == null)
                {
                    existingUser.ConnectionIds = new List<string>();
                }

                if (!existingUser.ConnectionIds.Contains(connectionId))
                {
                    existingUser.ConnectionIds.Add(connectionId);
                }

                existingUser.LastConnectedDate = DateTime.Now;
                _database.Store(existingUser);
                return new SingleResponse<User>
                {
                    Data = Mapper.Map<User>(existingUser),
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.None
                    }
                };
            }
            catch (Exception ex)
            {
                return new SingleResponse<User>
                {
                    Data = user,
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.Critical,
                        ErrorMessage = string.Format("Fatal error updating connection: {0}", "ARG0"),
                        ErrorException = ex
                    }
                };
            }
        }

        private RavenUser GetRavenUser(string id)
        {
            var storageUser = _database.Load<RavenUser>(id);
            return storageUser;
        }

        private RavenUser GetRavenUserByUsername(User user)
        {
            var storageUser = _database.Query<RavenUser>()
                                       .FirstOrDefault(u => u.Username == user.Username);
            return storageUser;
        }

        private Status ValidateNewUser(User user)
        {
            if (UserExists(user))
            {
                return new Status
                {
                    ErrorLevel = ErrorLevel.Error,
                    ErrorMessage = "User name already exists. Please enter a different user name."
                };
            }

            if (EmailExists(user))
            {
                return new Status
                {
                    ErrorLevel = ErrorLevel.Error,
                    ErrorMessage = "A user name for that e-mail already exists. Please enter a different e-mail."
                };
            }

            if (!PasswordValid(user))
            {
                return new Status
                {
                    ErrorLevel = ErrorLevel.Error,
                    ErrorMessage = "The password provided is invalid. " + _configuration.PasswordMessage
                };
            }

            if (!PasswordLengthValid(user))
            {
                return new Status
                {
                    ErrorLevel = ErrorLevel.Error,
                    ErrorMessage = "The password provided is invalid. Passwords must be greater than 6 characters. "
                };
            }

            if (!EmailValid(user))
            {
                return new Status
                    {
                        ErrorLevel = ErrorLevel.Error,
                        ErrorMessage = "The e-mail address provided is invalid. Please check the value and try again."
                    };
            }

            return new Status
            {
                ErrorLevel = ErrorLevel.None
            };
        }

        private bool UserExists(User user)
        {
            return _database.Query<RavenUser>()
                     .Any(u => u.Username == user.Username);
        }

        private bool EmailExists(User user)
        {
            return _database.Query<RavenUser>()
                            .Any(u => u.Email == user.Email);
        }

        private bool PasswordValid(User user)
        {
            string passwordRegex = _configuration.PasswordRegex;
            return Regex.IsMatch(user.Password, passwordRegex);
        }

        private bool PasswordLengthValid(User user)
        {
            return user.Password.Length >= 6;
        }

        private bool EmailValid(User user)
        {
            return Regex.IsMatch(user.Email, @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}\b");
        }
    }
}