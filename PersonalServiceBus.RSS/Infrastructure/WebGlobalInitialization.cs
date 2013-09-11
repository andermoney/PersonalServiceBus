using MassTransit;
using MassTransit.Log4NetIntegration;
using Ninject;
using PersonalServiceBus.RSS.Components.Feeds;
using PersonalServiceBus.RSS.Messages.Feeds;

namespace PersonalServiceBus.RSS.Infrastructure
{
    public static class WebGlobalInitialization
    {
        public static void InitializeBus()
        {
            Bus.Initialize(bus =>
            {
                bus.UseMsmq();
                bus.ReceiveFrom("msmq://localhost/personalservicebus.rss");
                bus.UseLog4Net();
                //TODO use container for this
                bus.Subscribe(subs =>
                {
                    subs.Handler<GetFeedItems>(h => NinjectRegistry.GetKernel().Get<GetFeedItemsProcessor>().Handle(h));
                    subs.Handler<AddFeed>(h => NinjectRegistry.GetKernel().Get<AddFeedProcessor>().Handle(h));
                });
            });
            //Set up scheduler
            Scheduler.Run();
        }
    }
}