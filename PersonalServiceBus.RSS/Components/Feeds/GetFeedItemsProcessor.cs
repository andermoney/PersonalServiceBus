using NServiceBus;
using PersonalServiceBus.RSS.Messages.Feeds;

namespace PersonalServiceBus.RSS.Components.Feeds
{
    public class GetFeedItemsProcessor : IHandleMessages<GetFeedItems>
    {
        public IBus Bus { get; set; }

        public void Handle(GetFeedItems message)
        {
        }
    }
}