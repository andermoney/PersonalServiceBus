using System;
using PersonalServiceBus.RSS.Core.Domain.Enum;

namespace PersonalServiceBus.RSS.Core.Domain.Model
{
    public class Status
    {
        public ErrorLevel ErrorLevel { get; set; }
        public string ErrorMessage { get; set; }
        public Exception ErrorException { get; set; }
    }
}