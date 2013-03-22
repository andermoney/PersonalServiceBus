using System;
using PersonalServiceBus.RSS.Messages.Videos;


namespace PersonalServiceBus.Email.Components.Videos
{
	public partial class AddVideoProcessor
	{
		
        partial void HandleImplementation(AddVideo message)
        {
            // Implement your handler logic here.
            Console.WriteLine("Videos received " + message.GetType().Name);
        }

	}
}