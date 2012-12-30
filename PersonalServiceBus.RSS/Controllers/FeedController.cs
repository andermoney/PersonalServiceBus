using System.Collections.Generic;
using System.Web.Http;
using PersonalServiceBus.RSS.Models;

namespace PersonalServiceBus.RSS.Controllers
{
    public class FeedController : ApiController
    {
        public IEnumerable<Feed> GetFeeds()
        {
            return new List<Feed>();
        }
    }
}
