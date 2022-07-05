using h5yr.Settings;
using h5yr.ViewComponents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tweetinvi;
using Tweetinvi.Models;
using Umbraco.Cms.Web.Common.Controllers;

namespace h5yr.Controllers
{

    [Route("api/loadmoretweets")]
    public class LoadMoreTweetsController : Controller
    {

        private readonly string? _consumerKey;
        private readonly string? _consumerSecret;
        private readonly string? _accessToken;
        private readonly string? _accessTokenSecret;


        public LoadMoreTweetsController(IOptions<TwitterSettings> twitterSettings)
        {
            var ts = twitterSettings.Value;
            if (ts != null)
            {
                _consumerKey = ts.ConsumerKey;
                _consumerSecret = ts.ConsumerSecret;
                _accessToken = ts.AccessToken;
                _accessTokenSecret = ts.AccessTokenSecret;
            }
        }

        [HttpGet]
        public IActionResult GetTweets()
        {
            var tweetsToSkip = Convert.ToInt32(HttpContext.Session.GetInt32("NumberOfTweetsDisplayed"));

            var list = GetAllTweets(tweetsToSkip, 12);

            HttpContext.Session.SetInt32("NumberOfTweetsDisplayed", tweetsToSkip+12);

            return View("Tweets/LoadMoreTweets", list);
        }

        private List<TweetModel> GetAllTweets(int tweetsToSkip, int tweetsToReturn)
        {
            // You need to make sure your app on dev.twitter.com has read and write permissions if you wish to tweet!
            var creds = new TwitterCredentials(_consumerKey, _consumerSecret, _accessToken, _accessTokenSecret);
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
        public List<TweetModel> GetMoreTweets()
        {

            var tweetsToSkip = Convert.ToInt32(HttpContext.Session.GetInt32("NumberOfTweetsDisplayed"));

            List<TweetModel> tweets = GetAllTweets(tweetsToSkip, 12);

            HttpContext.Session.SetInt32("NumberOfTweetsDisplayed", tweetsToSkip + 12);

            return tweets;
        }
    }
}

