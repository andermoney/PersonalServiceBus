using System;
using NServiceBus;
using PersonalServiceBus.Contract.Videos;


namespace PersonalServiceBus.RSS.Components.Videos
{
    public partial class FoundVideoProcessor : IHandleMessages<VideoFound>
    {
		
		public void Handle(VideoFound message)
		{
			this.HandleImplementation(message);
		}

		partial void HandleImplementation(VideoFound message);

		public IBus Bus { get; set; }

    }
}