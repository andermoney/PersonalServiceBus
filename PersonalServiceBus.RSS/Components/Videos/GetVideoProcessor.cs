using System;
using PersonalServiceBus.RSS.Messages.Videos;

namespace PersonalServiceBus.RSS.Components.Videos
{
    public class GetVideoProcessor// : IHandleMessages<GetVideo>
	{
        public void Handle(GetVideo message)
        {
            // Implement your handler logic here.
            Console.WriteLine("Videos received " + message.GetType().Name);
        }
	}
}