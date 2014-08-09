using ServiceStack;

namespace PersonalServiceBus.Hub.Messages
{
    [Route("/log", "GET")]
    [Route("/log/{Page}", "GET")]
    public class GetLogsRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}