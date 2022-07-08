using H5YR.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

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

            //  var json = JsonConvert.SerializeObject(tweets);
            //  string fileName = "TestTweets.json";
            //  string jsonString = System.Text.Json.JsonSerializer.Serialize(tweets);
            //  File.WriteAllText(fileName, jsonString);

            string fileName = "TestTweets.json";
            string jsonString = File.ReadAllText(fileName);
            List<TweetModel> tweet = System.Text.Json.JsonSerializer.Deserialize<List<TweetModel>>(jsonString)!;


            return View(tweet);
        }


        

    }
}

