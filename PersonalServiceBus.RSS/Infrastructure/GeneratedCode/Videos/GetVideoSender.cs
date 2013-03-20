using System;
using NServiceBus;
using PersonalServiceBus.InternalMessages.Videos;


namespace PersonalServiceBus.RSS.Components.Videos
{
    public partial class GetVideoSender 
    {
		

		public IBus Bus { get; set; }
		public void Send(GetVideo message)
		{
			Bus.Send(message);
		}


    }
}