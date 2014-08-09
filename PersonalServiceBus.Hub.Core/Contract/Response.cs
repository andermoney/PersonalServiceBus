using PersonalServiceBus.Hub.Core.Domain.Enum;

namespace PersonalServiceBus.Hub.Core.Contract
{
    public class Response
    {
        public ErrorLevel ErrorLevel { get; set; }
        public string Message { get; set; }
    }

    public class Response<T> : Response
    {
        public T Data { get; set; }
    }
}