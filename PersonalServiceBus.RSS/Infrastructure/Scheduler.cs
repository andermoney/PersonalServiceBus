using System;
using NServiceBus;
using NServiceBus.Unicast;
using PersonalServiceBus.RSS.Messages.Feeds;

namespace PersonalServiceBus.RSS.Infrastructure
{
    public class Scheduler : IWantToRunWhenTheBusStarts
    {
        public IBus Bus { get; set; }

        public void Run()
        {
            Schedule.Every(TimeSpan.FromMinutes(1)).Action(() => Bus.Send(new GetFeedItems()));
        }
    }
}