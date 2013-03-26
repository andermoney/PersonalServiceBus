using NServiceBus;
using PersonalServiceBus.RSS.Infrastructure;
using PersonalServiceBus.RSS.Messages.Feeds;

namespace PersonalServiceBus.RSS.Components.Feeds
{
    public class AddFeedSender : IAddFeedSender, INServiceBusComponent
    {
        public ICallback Send(AddFeed message)
        {
            return Bus.Send(message);
        }

        public IBus Bus { get; set; }
    }

    public interface IAddFeedSender
    {
        ICallback Send(AddFeed message);
    }
}