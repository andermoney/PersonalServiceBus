using System;
using System.Collections.Generic;
using PersonalServiceBus.RSS.Core.Contract;
using PersonalServiceBus.RSS.Core.Domain.Enum;
using PersonalServiceBus.RSS.Core.Domain.Model;

namespace PersonalServiceBus.RSS.Core.Helper
{
    public static class ResponseBuilder
    {
        public static SingleResponse<T> BuildSingleResponse<T>(ErrorLevel errorLevel, string errorMessage) where T : new()
        {
            return new SingleResponse<T>
            {
                Data = new T(),
                Status = new Status
                {
                    ErrorLevel = errorLevel,
                    ErrorMessage = errorMessage
                }
            };
        }

        public static SingleResponse<T> BuildSingleResponse<T>(T data, ErrorLevel errorLevel, string errorMessage)
        {
            return new SingleResponse<T>
                {
                    Data = data,
                    Status = new Status
                        {
                            ErrorLevel = errorLevel,
                            ErrorMessage = errorMessage
                        }
                };
        }

        public static SingleResponse<T> BuildSingleResponse<T>(T data, ErrorLevel errorLevel)
        {
            return new SingleResponse<T>
                {
                    Data = data,
                    Status = new Status
                        {
                            ErrorLevel = ErrorLevel.None
                        }
                };
        }

        public static SingleResponse<T> BuildSingleResponse<T>(T data, ErrorLevel errorLevel, string errorMessage, Exception ex)
        {
            return new SingleResponse<T>
                {
                    Data = data,
                    Status = new Status
                        {
                            ErrorLevel = errorLevel,
                            ErrorMessage = errorMessage,
                            ErrorException = ex
                        }
                };
        }

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

        public static CollectionResponse<T> BuildCollectionResponse<T>(IEnumerable<T> data, ErrorLevel errorLevel, string errorMessage)
        {
            return new CollectionResponse<T>
                {
                    Data = data,
                    Status = new Status
                        {
                            ErrorLevel = errorLevel,
                            ErrorMessage = errorMessage
                        }
                };
        }

        public static CollectionResponse<T> BuildCollectionResponse<T>(IEnumerable<T> data, ErrorLevel errorLevel)
        {
            return new CollectionResponse<T>
                {
                    Data = data,
                    Status = new Status
                        {
                            ErrorLevel = errorLevel
                        }
                };
        }

        public static CollectionResponse<T> BuildCollectionResponse<T>(ErrorLevel errorLevel, string errorMessage, Exception ex)
        {
            return new CollectionResponse<T>
                {
                    Data = new List<T>(),
                    Status = new Status
                        {
                            ErrorLevel = errorLevel,
                            ErrorMessage = errorMessage,
                            ErrorException = ex
                        }
                };
        }
    }
}