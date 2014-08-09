using System;
using PersonalServiceBus.Hub.Core.Domain.Enum;
using PersonalServiceBus.Hub.Core.Domain.Interface;
using PersonalServiceBus.Hub.Core.Domain.Model;
using PersonalServiceBus.Hub.Messages;
using ServiceStack;

namespace PersonalServiceBus.Hub.Services
{
    public class SubscribeService : Service
    {
        private readonly ILogger _logger;

        public SubscribeService(ILogger logger)
        {
            _logger = logger;
        }

        public SubscribeResponse Post(SubscribeRequest request)
        {
            _logger.Log(new LogEntry
            {
                Host = Request.UserHostAddress,
                Source = "SubscribeService",
                Description = string.Format("{0} subscribing to {1}", Request.UserHostAddress, request.Publisher)
            });
            return new SubscribeResponse
            {
                ErrorLevel = ErrorLevel.Error,
                Message = string.Format("Can't subscribe {0}", Request.UserHostAddress)
            };
        }
    }
}