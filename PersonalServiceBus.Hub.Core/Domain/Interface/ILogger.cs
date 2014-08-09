using PersonalServiceBus.Hub.Core.Contract;
using PersonalServiceBus.Hub.Core.Domain.Model;

namespace PersonalServiceBus.Hub.Core.Domain.Interface
{
    public interface ILogger
    {
        Response Log(LogEntry logEntry);
        Response GetLogs(PagedRequest request);
    }
}