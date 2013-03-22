using NServiceBus;
using PersonalServiceBus.RSS.Messages.Videos;


namespace PersonalServiceBus.RSS.Components.Videos
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