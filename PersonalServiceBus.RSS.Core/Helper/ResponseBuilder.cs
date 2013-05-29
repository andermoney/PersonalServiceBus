using System.Collections.Generic;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Helper
{
    public static class ResponseBuilder
    {
        public static CollectionResponse<T> BuildCollectionResponse<T>(ErrorLevel errorLevel, string errorMessage)
        {
            return new CollectionResponse<T>
                {
                    Data = new List<T>(),
                    Status = new Status
                        {
                            ErrorLevel = errorLevel,
                            ErrorMessage = errorMessage
                        }
                };
        }
    }
}