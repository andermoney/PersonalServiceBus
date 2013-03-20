using System;
using NServiceBus;
using PersonalServiceBus.Contract.Videos;


namespace PersonalServiceBus.RSS.Components.Videos
{
	public partial class FoundVideoProcessor
	{
		
        partial void HandleImplementation(VideoFound message)
        {
            // Implement your handler logic here.
            Console.WriteLine("Videos received " + message.GetType().Name);
        }

	}
}