using System;
using NServiceBus;
using PersonalServiceBus.InternalMessages.Videos;


namespace PersonalServiceBus.RSS1.Components.Videos
{
    public partial class GetVideoProcessor : IHandleMessages<GetVideo>
    {
		
		public void Handle(GetVideo message)
		{
			this.HandleImplementation(message);
		}

		partial void HandleImplementation(GetVideo message);

		public IBus Bus { get; set; }

    }
}