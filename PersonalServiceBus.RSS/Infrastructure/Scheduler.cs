using System;
using MassTransit;
using PersonalServiceBus.RSS.Messages.Feeds;

namespace PersonalServiceBus.RSS.Infrastructure
{
    public static class Scheduler
    {
        private static readonly System.Timers.Timer Timer = new System.Timers.Timer();

        private static void TimerElapsed()
        {
            //TODO make this a parameter
            Bus.Instance.Publish(new GetFeedItems());
        }

        public static void Run()
        {
            Timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            Timer.Elapsed += (sender, args) => TimerElapsed();
            Timer.Start();
        }
    }
}