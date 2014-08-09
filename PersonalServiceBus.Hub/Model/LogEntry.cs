using System;
using PersonalServiceBus.Hub.Enumeration;

namespace PersonalServiceBus.Hub.Model
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