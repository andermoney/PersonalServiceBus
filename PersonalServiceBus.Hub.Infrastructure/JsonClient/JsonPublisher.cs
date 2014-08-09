using PersonalServiceBus.Hub.Core.Contract;
using PersonalServiceBus.Hub.Core.Domain.Interface;
using PersonalServiceBus.Hub.Core.Domain.Model;

namespace PersonalServiceBus.Hub.Infrastructure.JsonClient
{
    public class JsonPublisher : IPublisher
    {
        public Response Publish(HubEvent hubEvent)
        {
            return new Response();
        }
    }
}