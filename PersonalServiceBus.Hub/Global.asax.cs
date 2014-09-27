using System;
using System.Collections.Generic;
using System.Threading;
using AutoMapper;
using Funq;
using PersonalServiceBus.Hub.Core.Contract;
using PersonalServiceBus.Hub.Core.Domain.Enum;
using PersonalServiceBus.Hub.Core.Domain.Interface;
using PersonalServiceBus.Hub.Core.Domain.Model;
using PersonalServiceBus.Hub.Infrastructure.JsonClient;
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
            private Timer _timer;

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
                RegisterAs<JsonPublisher, IPublisher>();
                RegisterAs<RavenDBSubscriber, ISubscriber>();

                Mapper.CreateMap<AddLogRequest, LogEntry>()
                    .ForMember(l => l.CreatedDate, opt => opt.MapFrom(d => DateTime.Now));
                Mapper.CreateMap<Response, AddLogResponse>();
                Mapper.CreateMap<Response<List<LogEntry>>, GetLogsResponse>();
                Mapper.CreateMap<GetLogsRequest, PagedRequest>();
                Mapper.CreateMap<SubscribeRequest, Subscription>();
                Mapper.CreateMap<Response, SubscribeResponse>();

                var publisher = TryResolve<IPublisher>();
                var logger = TryResolve<ILogger>();

                _timer = new Timer(obj =>
                {
                    logger.Log(new LogEntry
                    {
                        Host = "127.0.0.1",
                        Source = "Timer",
                        Description = string.Format("Timer fired at {0}", DateTime.Now)
                    });
                    publisher.Publish(new HubEvent
                    {
                        Host = "localhost",
                        Name = "Timer",
                        Source = "Timer"
                    });
                }, null, new TimeSpan(0, 0, 0), new TimeSpan(0, 1, 0));

                GlobalResponseFilters.Add((req, res, dto) =>
                {
                    var responseName = dto.GetType().Name.Replace("Response", "");
                    if (!string.IsNullOrEmpty(responseName))
                    {
                        string host = new Uri(req.AbsoluteUri).Host;
                        Response publishResponse = publisher.Publish(new HubEvent
                        {
                            Host = host,
                            Source = "Hub",
                            Name = responseName
                        });
                        if (publishResponse.ErrorLevel >= ErrorLevel.Warning)
                        {
                            logger.Log(new LogEntry
                            {
                                Host = host,
                                Source = "Hub",
                                Description = string.Format("Tried to publish message {0}: {1}", responseName, publishResponse.Message),
                                ErrorLevel = publishResponse.ErrorLevel
                            });
                        }
                        else
                        {
                            logger.Log(new LogEntry
                            {
                                Host = host,
                                Source = "Hub",
                                Description = string.Format("Published {0}", responseName)
                            });
                        }
                    }
                });
            }

            public override void OnUncaughtException(IRequest httpReq, IResponse httpRes, string operationName, Exception ex)
            {
                var logger = TryResolve<ILogger>();
                if (logger != null)
                {
                    logger.Log(new LogEntry
                    {
                        Host = httpReq.UserHostAddress,
                        Source = "UncaughtException",
                        Description = ex.ToString()
                    });
                }
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