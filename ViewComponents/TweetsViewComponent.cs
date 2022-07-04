using Microsoft.AspNetCore.Mvc;
using Tweetinvi;
using Tweetinvi.Models;

namespace h5yr.ViewComponents
{
    public class TweetsViewComponent : ViewComponent
    {
        private readonly IConfiguration Configuration;

        public TweetsViewComponent(IConfiguration config)
        {
            Configuration = config;
        }
        private string consumerKey => Configuration["TWITTER_CONSUMER_KEY"];
        private string consumerSecret => Configuration["TWITTER_CONSUMER_SECRET"];
        private string accessToken => Configuration["TWITTER_ACCESS_TOKEN"];
        private string accessTokenSecret => Configuration["TWITTER_ACCESS_TOKEN_SECRET"];


        public IViewComponentResult Invoke(string loadmore = "false")
        {

            HttpContext.Session.SetInt32("NumberOfTweetsDisplayed", 12);

            List<TweetModel> tweets;

            if (loadmore == "false")
            {
             tweets = GetAllTweets(0, 12);

            }
            else
            {
                tweets = GetMoreTweets();
            }

            return View(tweets);
        }


        private List<TweetModel> GetAllTweets(int tweetsToSkip, int tweetsToReturn)
        {
            // You need to make sure your app on dev.twitter.com has read and write permissions if you wish to tweet!
            var creds =new TwitterCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);
            var userClient = new TwitterClient(creds);

            var searchResults = userClient.Search.SearchTweetsAsync("#h5yr");


            List<TweetModel> FetchTweets = new List<TweetModel>();


            foreach (var tweet in searchResults.Result.Skip(tweetsToSkip).Take(tweetsToReturn))
            {
                FetchTweets.Add(new TweetModel()
                {

                    Username = tweet.CreatedBy.ToString(),
                    Avatar = tweet.CreatedBy.ProfileImageUrl,
                    Content = tweet.Text,
                    ScreenName = tweet.CreatedBy.ScreenName.ToString(),
                    TweetedOn = tweet.CreatedAt,
                    NumberOfTweets = FetchTweets.Count(),
                    ReplyToTweet = tweet.IdStr,
                    Url = tweet.Url

                });


            };

            return FetchTweets;
        }



        [HttpGet]
        public List<TweetModel> GetMoreTweets()
        {

            var tweetsToSkip = Convert.ToInt32(HttpContext.Session.GetInt32("NumberOfTweetsDisplayed"));

            List<TweetModel> tweets = GetAllTweets(tweetsToSkip, 12);

            HttpContext.Session.SetInt32("NumberOfTweetsDisplayed",tweetsToSkip + 12);

            return tweets;
        }
    }

   


}

