using System;
using AutoMapper;
using Funq;
using PersonalServiceBus.Hub.Core.Contract;
using PersonalServiceBus.Hub.Core.Domain.Interface;
using PersonalServiceBus.Hub.Core.Domain.Model;
using PersonalServiceBus.Hub.Infrastructure.RavenDB;
using PersonalServiceBus.Hub.Messages;
using PersonalServiceBus.Hub.Services;
using Raven.Client.Document;
using ServiceStack;
using ServiceStack.Web;

namespace PersonalServiceBus.Hub
{
    public class Global : System.Web.HttpApplication
    {
        public class AppHost : AppHostBase
        {
            private DocumentStore _documentStore;
            //Tell ServiceStack the name of your application and where to find your services
            public AppHost() : base("Subscription Web Service", typeof(SubscribeService).Assembly) { }

            public override void Configure(Container container)
            {
                _documentStore = new DocumentStore
                {
                    Url = "http://localhost:8080",
                    DefaultDatabase = "PersonalServiceBus.Log"
                };
                _documentStore.Initialize();
                Register(_documentStore);
                RegisterAs<RavenDBLogger, ILogger>();

                Mapper.CreateMap<AddLogRequest, LogEntry>()
                    .ForMember(l => l.CreatedDate, opt => opt.MapFrom(d => DateTime.Now));
                Mapper.CreateMap<Response, AddLogResponse>();
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