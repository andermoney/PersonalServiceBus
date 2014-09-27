using System.Collections.Generic;
using PersonalServiceBus.Hub.Core.Contract;
using PersonalServiceBus.Hub.Core.Domain.Model;

namespace PersonalServiceBus.Hub.Messages
{
    public class GetLogsResponse : Response<List<LogEntry>>
    {
         
    }
}