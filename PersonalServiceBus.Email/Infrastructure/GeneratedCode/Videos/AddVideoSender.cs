using System;
using NServiceBus;
using NServiceBus.Config;
using PersonalServiceBus.InternalMessages.Videos;

namespace PersonalServiceBus.Email.Components.Videos
{
    public partial class AddVideoSender: IAddVideoSender, PersonalServiceBus.Email.Infrastructure.INServiceBusComponent
    {
        public void Send(AddVideo message)
		{
			Bus.Send(message);	
		}

        public IBus Bus { get; set; }
    }

    public interface IAddVideoSender
    {
        void Send(AddVideo message);
    }
}