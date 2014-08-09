using System;
using AutoMapper;
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
        private readonly ISubscriber _subscriber;

        public SubscribeService(ILogger logger,
            ISubscriber subscriber)
        {
            _logger = logger;
            _subscriber = subscriber;
        }

        public SubscribeResponse Post(SubscribeRequest request)
        {
            _logger.Log(new LogEntry
            {
                Host = Request.RemoteIp,
                Source = "SubscribeService",
                Description = string.Format("{0} subscribing to {1}", Request.RemoteIp, request.Publisher)
            });
            var subscription = Mapper.Map<Subscription>(request);
            if (string.IsNullOrEmpty(subscription.Host))
            {
                subscription.Host = Request.RemoteIp;
            }
            return Mapper.Map<SubscribeResponse>(_subscriber.Subscribe(subscription));
        }
    }
}