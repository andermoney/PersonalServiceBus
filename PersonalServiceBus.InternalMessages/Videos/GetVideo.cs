using System;
using NServiceBus;
using NServiceBus.Config;
using PersonalServiceBus.InternalMessages.Videos;

namespace PersonalServiceBus.RSS1.Components.Videos
{
    public partial class GetVideoSender: IGetVideoSender, PersonalServiceBus.RSS1.Infrastructure.INServiceBusComponent
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