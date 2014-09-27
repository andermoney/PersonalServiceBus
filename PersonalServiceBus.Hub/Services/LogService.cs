using System;
using AutoMapper;
using PersonalServiceBus.Hub.Core.Contract;
using PersonalServiceBus.Hub.Core.Domain.Enum;
using PersonalServiceBus.Hub.Core.Domain.Interface;
using PersonalServiceBus.Hub.Core.Domain.Model;
using PersonalServiceBus.Hub.Messages;
using ServiceStack;

namespace PersonalServiceBus.Hub.Services
{
    public class LogService : Service
    {
        private readonly ILogger _logger;

        public LogService(ILogger logger)
        {
            _logger = logger;
        }

        public GetLogsResponse Get(GetLogsRequest request)
        {
            try
            {
                if (request.PageSize == 0)
                {
                    request.PageSize = 10;
                }
                return Mapper.Map<GetLogsResponse>(_logger.GetLogs(Mapper.Map<PagedRequest>(request)));
            }
            catch (Exception ex)
            {
                return new GetLogsResponse
                {
                    ErrorLevel = ErrorLevel.Critical,
                    Message = ex.ToString()
                };
            }
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