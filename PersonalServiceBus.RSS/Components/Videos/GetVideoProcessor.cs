using System;
using PersonalServiceBus.RSS.Messages.Videos;


namespace PersonalServiceBus.RSS.Components.Videos
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