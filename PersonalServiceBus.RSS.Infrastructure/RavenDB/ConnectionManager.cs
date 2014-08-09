using System.Linq;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Core.Helper;

namespace PersonalServiceBus.RSS.Infrastructure.RavenDB
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly IDatabase _database;

        public ConnectionManager(IDatabase database)
        {
            _database = database;
        }

        public SingleResponse<Connection> AddConnection(Connection connection)
        {
            connection.Id = _database.Store(connection);
            return ResponseBuilder.BuildSingleResponse(connection, ErrorLevel.None);
        }

        public CollectionResponse<Connection> GetAllConnections()
        {
            return ResponseBuilder.BuildCollectionResponse(_database.Query<Connection>().ToList(), ErrorLevel.None);
        }
    }
}