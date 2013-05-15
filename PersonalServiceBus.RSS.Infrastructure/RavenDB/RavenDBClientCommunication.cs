using System;
using System.Linq;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Infrastructure.RavenDB
{
    public class RavenDBClientCommunication : IClientCommunication
    {
        private readonly IDatabase _database;

        public RavenDBClientCommunication(IDatabase database)
        {
            _database = database;
        }

        public SingleResponse<ClientConnection> GetConnectionByUsername(ClientConnection clientConnection)
        {
            try
            {
                var connection = _database.Query<ClientConnection>()
                                          .FirstOrDefault(c => c.Username == clientConnection.Username);
                return new SingleResponse<ClientConnection>
                    {
                        Data = connection,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.None
                            }
                    };
            }
            catch (Exception ex)
            {
                return new SingleResponse<ClientConnection>
                    {
                        Data = clientConnection,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Critical,
                                ErrorMessage = string.Format("Fatal error getting client connection: {0}", ex),
                                ErrorException = ex
                            }
                    };
            }
        }

        public SingleResponse<ClientConnection> AddConnection(ClientConnection clientConnection)
        {
            try
            {
                if (clientConnection.ConnectionIds.Count != 1)
                {
                    throw new ArgumentException("Only adding one connection id at a time is supported");
                }

                var connectionResponse = GetConnectionByUsername(clientConnection);
                var connection = connectionResponse.Data;
                if (connection == null)
                {
                    connection = clientConnection;
                }
                else
                {
                    var connectionId = clientConnection.ConnectionIds[0];
                    if (!connection.ConnectionIds.Contains(connectionId))
                    {
                        connection.ConnectionIds.Add(connectionId);
                    }
                }
                connection.CreatedDate = DateTime.Now;
                _database.Store(connection);
                return new SingleResponse<ClientConnection>
                    {
                        Data = clientConnection,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.None
                            }
                    };
            }
            catch (Exception ex)
            {
                return new SingleResponse<ClientConnection>
                    {
                        Data = clientConnection,
                        Status = new Status
                            {
                                ErrorLevel = ErrorLevel.Critical,
                                ErrorMessage = string.Format("Fatal error adding connection: {0}", "ARG0"),
                                ErrorException = ex
                            }
                    };
            }
        }

        public SingleResponse<ClientConnection> RemoveConnection(ClientConnection clientConnection)
        {
            try
            {
                if (clientConnection.ConnectionIds.Count != 1)
                {
                    throw new ArgumentException("Only removing one connection id at a time is supported");
                }

                var connectionResponse = GetConnectionByUsername(clientConnection);
                var connection = connectionResponse.Data;
                var connectionId = clientConnection.ConnectionIds[0];
                if (connection.ConnectionIds.Contains(connectionId))
                {
                    connection.ConnectionIds.Remove(connectionId);
                }
                if (connection.ConnectionIds.Count == 0)
                {
                    _database.Delete(connection);
                }
                else
                {
                    connection.CreatedDate = DateTime.Now;
                    _database.Store(connection);
                }
                return new SingleResponse<ClientConnection>
                {
                    Data = clientConnection,
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.None
                    }
                };
            }
            catch (Exception ex)
            {
                return new SingleResponse<ClientConnection>
                {
                    Data = clientConnection,
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.Critical,
                        ErrorMessage = string.Format("Fatal error removing connection: {0}", "ARG0"),
                        ErrorException = ex
                    }
                };
            }
        }

        public SingleResponse<ClientConnection> UpdateConnection(ClientConnection clientConnection)
        {
            try
            {
                if (clientConnection.ConnectionIds.Count != 1)
                {
                    throw new ArgumentException("Only updating one connection id at a time is supported");
                }

                var connectionResponse = GetConnectionByUsername(clientConnection);
                var connection = connectionResponse.Data;
                if (connection == null)
                {
                    connection = clientConnection;
                }
                else
                {
                    var connectionId = clientConnection.ConnectionIds[0];
                    if (!connection.ConnectionIds.Contains(connectionId))
                    {
                        connection.ConnectionIds.Add(connectionId);
                    }
                }
                connection.CreatedDate = DateTime.Now;
                _database.Store(connection);
                return new SingleResponse<ClientConnection>
                {
                    Data = clientConnection,
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.None
                    }
                };
            }
            catch (Exception ex)
            {
                return new SingleResponse<ClientConnection>
                {
                    Data = clientConnection,
                    Status = new Status
                    {
                        ErrorLevel = ErrorLevel.Critical,
                        ErrorMessage = string.Format("Fatal error updating connection: {0}", "ARG0"),
                        ErrorException = ex
                    }
                };
            }
        }
    }
}