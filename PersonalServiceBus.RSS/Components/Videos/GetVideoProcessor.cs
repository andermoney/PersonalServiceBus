using System;
using NServiceBus;
using PersonalServiceBus.InternalMessages.Videos;


namespace PersonalServiceBus.RSS1.Components.Videos
{
	public partial class GetVideoProcessor
	{
		
        partial void HandleImplementation(GetVideo message)
        {
            // Implement your handler logic here.
            Console.WriteLine("Videos received " + message.GetType().Name);
        }

	}
}