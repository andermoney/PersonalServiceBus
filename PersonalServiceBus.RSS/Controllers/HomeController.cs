using System.Web.Mvc;
using PersonalServiceBus.RSS.Components.Feeds;
using PersonalServiceBus.RSS.Components.Videos;
using PersonalServiceBus.RSS.Messages.Feeds;
using PersonalServiceBus.RSS.Messages.Videos;
using PersonalServiceBus.RSS.Models;

namespace PersonalServiceBus.RSS.Controllers
{
    public class HomeController : Controller
    {
        public IGetVideoSender GetVideoSender { get; set; }
        public IAddFeedSender AddFeedSender { get; set; }

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";

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

        public ActionResult AddFeed()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddFeed(AddFeedViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            AddFeedResponse response = new AddFeedResponse();
            AddFeedSender.Send(new AddFeed()).Register<AddFeedResponse>(r =>
                {
                    response = r;
                });

            ViewBag.Response = response;
            return RedirectToAction("Index");
        }
    }
}
