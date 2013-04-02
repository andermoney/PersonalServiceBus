using System.Web.Mvc;
using PersonalServiceBus.RSS.Components.Feeds;
using PersonalServiceBus.RSS.Components.Videos;
using PersonalServiceBus.RSS.Messages.Feeds;
using PersonalServiceBus.RSS.Messages.Videos;
using PersonalServiceBus.RSS.Models;

namespace PersonalServiceBus.RSS.Controllers
{
    public class HomeController : AsyncController
    {
        public IGetVideoSender GetVideoSender { get; set; }
        public IAddFeedSender AddFeedSender { get; set; }

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

        public ActionResult AddFeed()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AsyncTimeout(50000)]
        public void AddFeedAsync(AddFeedViewModel model)
        {
            if (!ModelState.IsValid)
                return;

            var response = AddFeedSender.Send(new AddFeed());

            AsyncManager.Parameters["errorMessage"] = response.ErrorMessage;
        }

        public ActionResult AddFeedCompleted(string errorMessage)
        {
            if (!ModelState.IsValid)
                return View();

            ViewBag.ErrorMessage = errorMessage;
            return RedirectToAction("Index");            
        }
    }
}
