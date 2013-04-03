using System.Web.Routing;
using Microsoft.AspNet.SignalR;
using PersonalServiceBus.RSS.App_Start;

[assembly: WebActivator.PreApplicationStartMethod(typeof(RegisterHubs), "Start")]

namespace PersonalServiceBus.RSS.App_Start
{
    public static class RegisterHubs
    {
        public static void Start()
        {
            // Register the default hubs route: ~/signalr/hubs
            RouteTable.Routes.MapHubs();
        }
    }
}
