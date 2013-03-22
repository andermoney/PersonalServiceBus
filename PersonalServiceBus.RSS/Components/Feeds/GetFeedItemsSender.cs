using NServiceBus;
using PersonalServiceBus.RSS.Infrastructure;
using PersonalServiceBus.RSS.Messages.Feeds;

namespace PersonalServiceBus.RSS.Components.Feeds
{
    public class GetFeedItemsSender : IGetFeedItemsSender, INServiceBusComponent
    {
        public void Send(GetFeedItems message)
        {
            Bus.Send(message);
        }

        public IBus Bus { get; set; }
    }

    public interface IGetFeedItemsSender
    {
        void Send(GetFeedItems message);
    }
}