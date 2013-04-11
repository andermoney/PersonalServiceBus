using System.Collections.Generic;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Contract
{
    public class CollectionResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public Status Status { get; set; }
    }
}