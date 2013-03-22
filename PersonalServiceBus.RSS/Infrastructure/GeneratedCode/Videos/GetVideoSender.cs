using NServiceBus;
using PersonalServiceBus.RSS.Messages.Videos;

namespace PersonalServiceBus.RSS.Components.Videos
{
    public partial class GetVideoSender: IGetVideoSender, PersonalServiceBus.RSS.Infrastructure.INServiceBusComponent
    {
        public void Send(GetVideo message)
        {
			Bus.Send(message);	
		}

        public IBus Bus { get; set; }
    }

    public interface IGetVideoSender
    {
        void Send(GetVideo message);
    }
}