using System;
using NServiceBus;
using PersonalServiceBus.RSS.Infrastructure;
using PersonalServiceBus.RSS.Messages.Feeds;

namespace PersonalServiceBus.RSS.Components.Feeds
{
    public class AddFeedSender : IAddFeedSender, INServiceBusComponent
    {
        public AddFeedResponse Send(AddFeed message)
        {
            IAsyncResult asyncResult = Bus.Send(message).Register(SendCallback, this);
            asyncResult.AsyncWaitHandle.WaitOne(50000);
            return new AddFeedResponse
                {
                    IsError = !string.IsNullOrEmpty(ErrorMessage),
                    ErrorMessage = ErrorMessage
                };
        }

        private void SendCallback(IAsyncResult asyncResult)
        {
            var result = asyncResult.AsyncState as CompletionResult;
            var sender = result.State as AddFeedSender;
            if (result.ErrorCode > 0)
            {
                sender.ErrorMessage = result.GetHeader("ErrorMessage");
            }
        }

        protected string ErrorMessage { get; set; }

        public IBus Bus { get; set; }
    }

    public interface IAddFeedSender
    {
        AddFeedResponse Send(AddFeed message);
    }
}