using System.Web.Mvc;
using PersonalServiceBus.InternalMessages.Videos;
using PersonalServiceBus.RSS.Components.Videos;

namespace PersonalServiceBus.RSS.Controllers
{
    public class HomeController : Controller
    {
        public IGetVideoSender GetVideoSender { get; set; }

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
    }
}
