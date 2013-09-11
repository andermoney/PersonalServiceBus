using System;
using PersonalServiceBus.Contract.Videos;

namespace PersonalServiceBus.RSS.Components.Videos
{
	public class FoundVideoProcessor
	{
        public void Handle(VideoFound message)
        {
            // Implement your handler logic here.
            Console.WriteLine("Videos received " + message.GetType().Name);
        }
	}
}