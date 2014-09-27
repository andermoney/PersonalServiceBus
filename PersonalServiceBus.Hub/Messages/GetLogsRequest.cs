using PersonalServiceBus.Hub.Core.Contract;
using ServiceStack;

namespace PersonalServiceBus.Hub.Messages
{
    [Route("/log", "GET")]
    [Route("/log/{Page}", "GET")]
    public class GetLogsRequest : PagedRequest
    {
    }
}