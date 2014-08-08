using PersonalServiceBus.Hub.Enumeration;
using PersonalServiceBus.Hub.Messages;
using ServiceStack;

namespace PersonalServiceBus.Hub.Services
{
    public class AddLogService : Service
    {
        public AddLogResponse Post(AddLogRequest request)
        {
            return new AddLogResponse
            {
                ErrorLevel = ErrorLevel.Critical,
                Message = "Not implemented yet"
            };
        }
    }
}