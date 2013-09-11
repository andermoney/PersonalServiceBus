using System.Web.Mvc;

namespace PersonalServiceBus.RSS.Controllers
{
    public class HomeController : AsyncController
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}
