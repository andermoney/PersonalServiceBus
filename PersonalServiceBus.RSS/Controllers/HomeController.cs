using System.Web.Mvc;
using PersonalServiceBus.RSS.Components.Feeds;
using PersonalServiceBus.RSS.Components.Videos;
using PersonalServiceBus.RSS.Messages.Videos;

namespace PersonalServiceBus.RSS.Controllers
{
    public class HomeController : AsyncController
    {
        public IGetVideoSender GetVideoSender { get; set; }
        public IAddFeedSender AddFeedSender { get; set; }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult GetVideo()
        {
            GetVideoSender.Send(new GetVideo());
            return RedirectToAction("Index");
        }
    }
}
