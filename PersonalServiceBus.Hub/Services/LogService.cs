using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PersonalServiceBus.Hub.Enumeration;
using PersonalServiceBus.Hub.Messages;
using PersonalServiceBus.Hub.Model;
using Raven.Client.Document;
using Raven.Client.Linq;
using ServiceStack;

namespace PersonalServiceBus.Hub.Services
{
    public class LogService : Service
    {
        private readonly DocumentStore _documentStore;

        public LogService(DocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public GetLogsResponse Get(GetLogsRequest request)
        {
            List<LogEntry> results;
            try
            {
                if (request.PageSize == 0)
                {
                    request.PageSize = 10;
                }
                using (var documentSession = _documentStore.OpenSession())
                {
                    results = documentSession.Query<LogEntry>()
                        .Skip(request.Page*request.PageSize)
                        .Take(request.PageSize)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                return new GetLogsResponse
                {
                    ErrorLevel = ErrorLevel.Critical,
                    Message = ex.ToString()
                };
            }
            return new GetLogsResponse
            {
                Data = results
            };
        }

        public AddLogResponse Post(AddLogRequest request)
        {
            try
            {
                using (var documentSession = _documentStore.OpenSession())
                {
                    documentSession.Store(Mapper.Map<LogEntry>(request));
                    //var id = documentSession.Advanced.GetDocumentId(request);
                    documentSession.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                return new AddLogResponse
                {
                    ErrorLevel = ErrorLevel.Critical,
                    Message = ex.ToString()
                };
            }
            return new AddLogResponse();
        }
    }
}