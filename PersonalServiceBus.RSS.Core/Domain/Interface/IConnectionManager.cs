using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IConnectionManager
    {
        SingleResponse<Connection> AddConnection(Connection connection);
        CollectionResponse<Connection> GetAllConnections();
    }
}