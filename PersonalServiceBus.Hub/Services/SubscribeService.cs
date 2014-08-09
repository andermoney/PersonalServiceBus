using PersonalServiceBus.Hub.Core.Domain.Enum;
using PersonalServiceBus.Hub.Messages;
using ServiceStack;

namespace PersonalServiceBus.Hub.Services
{
    public class SubscribeService : Service
    {
        public object Any(SubscribeRequest request)
        {
            return new SubscribeResponse
            {
                ErrorLevel = ErrorLevel.Error,
                Message = string.Format("Can't subscribe {0}", Request.UserHostAddress)
            };
        }
    }
}