using H5YR.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace h5yr.ViewComponents
{
    public class TweetsViewComponent : ViewComponent
    {
        private readonly ITwitterHelper twitterHelper;

        public TweetsViewComponent(ITwitterHelper twitterHelper)
        {
            this.twitterHelper = twitterHelper;
        }

        public IViewComponentResult Invoke(string loadmore = "false")
        {

            HttpContext.Session.SetInt32("NumberOfTweetsDisplayed", 12);

            List<TweetModel> tweets;

            tweets = twitterHelper.GetAllTweets(0, 12);

            return View(tweets);
        }


        

    }
}

