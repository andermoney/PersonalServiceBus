using System.Web.Http;
using PersonalServiceBus.Email.Components.Videos;
using PersonalServiceBus.InternalMessages.Videos;

namespace PersonalServiceBus.Email.Controllers
{
    public class VideoController : ApiController
    {
        public IAddVideoSender AddVideoSender { get; set; }

        public void AddVideo(AddVideo message)
        {
            AddVideoSender.Send(message);
        }
    }
}
