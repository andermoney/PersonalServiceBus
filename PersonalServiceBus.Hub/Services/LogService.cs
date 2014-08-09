using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PersonalServiceBus.Hub.Core.Contract;
using PersonalServiceBus.Hub.Core.Domain.Enum;
using PersonalServiceBus.Hub.Core.Domain.Interface;
using PersonalServiceBus.Hub.Core.Domain.Model;
using PersonalServiceBus.Hub.Messages;
using Raven.Client.Document;
using Raven.Client.Linq;
using ServiceStack;

namespace PersonalServiceBus.Hub.Services
{
    public class LogService : Service
    {
        private readonly DocumentStore _documentStore;
        private readonly ILogger _logger;

        public LogService(DocumentStore documentStore, ILogger logger)
        {
            _documentStore = documentStore;
            _logger = logger;
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
            Response response;
            try
            {
                response = _logger.Log(Mapper.Map<LogEntry>(request));
            }
            catch (Exception ex)
            {
                return new AddLogResponse
                {
                    ErrorLevel = ErrorLevel.Critical,
                    Message = ex.ToString()
                };
            }
            return Mapper.Map<AddLogResponse>(response);
        }
    }
}