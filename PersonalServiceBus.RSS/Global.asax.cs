using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Funq;
using PersonalServiceBus.RSS.Components.Feeds;
using PersonalServiceBus.RSS.Core.Domain.Interface;
using PersonalServiceBus.RSS.Infrastructure;
using PersonalServiceBus.RSS.Infrastructure.Cryptography;
using PersonalServiceBus.RSS.Infrastructure.RavenDB;
using PersonalServiceBus.RSS.Infrastructure.RSS;
using PersonalServiceBus.RSS.Services;
using PersonalServiceBus.RSS.SignalR;
using ServiceStack;
using ServiceStack.Web;

namespace PersonalServiceBus.RSS
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public class AppHost : AppHostBase
        {
            //private DocumentStore _documentStore;
            private Timer _timer;

            public AppHost() : base("RSS Web Service", typeof(RSSService).Assembly) { }

            public override void Configure(Container container)
            {
                SetConfig(new HostConfig { HandlerFactoryPath = "api" });
                RegisterAs<RavenDatabase, IDatabase>();
                RegisterAs<WebConfiguration, IConfiguration>();
                RegisterAs<Cryptography, ICryptography>();
                RegisterAs<FeedItemsProcessor, IFeedItemsProcessor>();
                RegisterAs<FeedManager, IFeedManager>();
                RegisterAs<RssManager,IRssManager>();
                RegisterAs<RavenDBAuthentication, IAuthentication>();
                Register(new FeedHubClient());
            }

            public override void OnUncaughtException(IRequest httpReq, IResponse httpRes, string operationName, Exception ex)
            {
                //var logger = TryResolve<ILogger>();
                //if (logger != null)
                //{
                //    logger.Log(new LogEntry
                //    {
                //        Host = httpReq.UserHostAddress,
                //        Source = "UncaughtException",
                //        Description = ex.ToString()
                //    });
                //}
                base.OnUncaughtException(httpReq, httpRes, operationName, ex);
            }

            public override bool ApplyMessageResponseFilters(IRequest req, IResponse res, object response)
            {
                return base.ApplyMessageResponseFilters(req, res, response);
            }

            public override object OnPostExecuteServiceFilter(IService service, object response, IRequest httpReq, IResponse httpRes)
            {
                return base.OnPostExecuteServiceFilter(service, response, httpReq, httpRes);
            }

            public override bool ApplyRequestFilters(IRequest req, IResponse res, object requestDto)
            {
                return base.ApplyRequestFilters(req, res, requestDto);
            }
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("api/{*pathInfo}");
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            new AppHost().Init();

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            WebGlobalInitialization.InitializeBus();
        }
    }
}