using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Contract
{
    public class SingleResponse<T>
    {
        public T Data { get; set; }
        public Status Status { get; set; }
    }
}