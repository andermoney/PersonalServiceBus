﻿using PersonalServiceBus.Hub.Enumeration;

namespace PersonalServiceBus.Hub.Messages
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