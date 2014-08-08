using System;
using Funq;
using PersonalServiceBus.Hub.Services;
using ServiceStack;
using ServiceStack.Web;

namespace PersonalServiceBus.Hub
{
    public class Global : System.Web.HttpApplication
    {
        public class AppHost : AppHostBase
        {
            //Tell ServiceStack the name of your application and where to find your services
            public AppHost() : base("Subscription Web Service", typeof(SubscribeService).Assembly) { }

            public override void Configure(Container container)
            {
                
            }

            public override void OnUncaughtException(IRequest httpReq, IResponse httpRes, string operationName, Exception ex)
            {
                base.OnUncaughtException(httpReq, httpRes, operationName, ex);
            }
        }
        protected void Application_Start(object sender, EventArgs e)
        {
            new AppHost().Init();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}