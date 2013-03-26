using NServiceBus;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Core.Domain.Model;
using PersonalServiceBus.RSS.Messages.Feeds;

namespace PersonalServiceBus.RSS.Components.Feeds
{
    public class AddFeedProcessor : IHandleMessages<AddFeed>
    {
        public IBus Bus { get; set; }

        private readonly IFeedManager _feedManager;

        public AddFeedProcessor(IFeedManager feedManager)
        {
            _feedManager = feedManager;
        }

        public void Handle(AddFeed message)
        {
            var status = _feedManager.AddFeed(new Feed());
            Bus.Reply(new AddFeedResponse
                {
                    IsError = status.ErrorLevel > ErrorLevel.Warning,
                    ErrorMessage = status.ErrorMessage,
                    ErrorException = status.ErrorException
                });
        }
    }
}