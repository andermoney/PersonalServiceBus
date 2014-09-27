using PersonalServiceBus.Hub.Core.Contract;
using PersonalServiceBus.Hub.Core.Domain.Model;

namespace PersonalServiceBus.Hub.Core.Domain.Interface
{
    public interface ISubscriber
    {
        Response Subscribe(Subscription subscription);
    }
}