using NServiceBus;
using PersonalServiceBus.RSS.Messages.Videos;


namespace PersonalServiceBus.Email.Components.Videos
{
    public partial class AddVideoProcessor : IHandleMessages<AddVideo>
    {
		
		public void Handle(AddVideo message)
		{
			this.HandleImplementation(message);
		}

		partial void HandleImplementation(AddVideo message);

		public IBus Bus { get; set; }

    }
}