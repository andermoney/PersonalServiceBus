using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Domain.Interface
{
    public interface IClientCommunication
    {
        SingleResponse<ClientConnection> AddConnection(ClientConnection clientConnection);
        SingleResponse<ClientConnection> RemoveConnection(ClientConnection clientConnection);
        SingleResponse<ClientConnection> UpdateConnection(ClientConnection clientConnection);
        SingleResponse<ClientConnection> GetConnectionByUsername(ClientConnection clientConnection);
    }
}