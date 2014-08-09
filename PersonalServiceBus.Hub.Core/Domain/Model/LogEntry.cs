using System;
using PersonalServiceBus.Hub.Core.Domain.Enum;

namespace PersonalServiceBus.Hub.Core.Domain.Model
{
    public class LogEntry
    {
        public string Source { get; set; }
        public string Host { get; set; }
        public ErrorLevel ErrorLevel { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}