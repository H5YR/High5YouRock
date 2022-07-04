using Microsoft.AspNetCore.Mvc;

namespace h5yr.Controllers
{
    public class LoadMoreTweetsController : Controller
    {
        public IActionResult Index()
        {
            return ViewComponent("Tweets", new {loadmore = "yes"});
        }
    }
}
