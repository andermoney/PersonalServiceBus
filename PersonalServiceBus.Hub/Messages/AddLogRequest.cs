using PersonalServiceBus.Hub.Core.Domain.Enum;
using ServiceStack;

namespace PersonalServiceBus.Hub.Messages
{
    [Route("/log", "POST")]
    public class AddLogRequest
    {
        public string Source { get; set; }
        public string Host { get; set; }
        public ErrorLevel ErrorLevel { get; set; }
        public string Description { get; set; }
    }
}