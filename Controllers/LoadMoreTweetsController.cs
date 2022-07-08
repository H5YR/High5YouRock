using h5yr.Settings;
using h5yr.ViewComponents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tweetinvi;
using Tweetinvi.Models;


namespace h5yr.Controllers
{

    [Route("api/loadmoretweets")]
    public class LoadMoreTweetsController : Controller
    {

        private readonly string? _consumerKey;
        private readonly string? _consumerSecret;
        private readonly string? _accessToken;
        private readonly string? _accessTokenSecret;
        private readonly ILogger<LoadMoreTweetsController> _logger;
        private readonly IOptions<APISettings> _apiSettings;


        public LoadMoreTweetsController(IOptions<TwitterSettings> twitterSettings, ILogger<LoadMoreTweetsController> logger, IOptions<APISettings> apiSettings)
        {
            var ts = twitterSettings.Value;
            if (ts != null)
            {
                _consumerKey = ts.ConsumerKey;
                _consumerSecret = ts.ConsumerSecret;
                _accessToken = ts.AccessToken;
                _accessTokenSecret = ts.AccessTokenSecret;
            }
            _logger = logger;
            _apiSettings = apiSettings;
        }

        [HttpGet]
        public IActionResult GetTweets()
        {
            var tweetsToSkip = Convert.ToInt32(HttpContext.Session.GetInt32("NumberOfTweetsDisplayed"));
            List<TweetModel> tweets = new();

            if (_apiSettings.Value.Offline == null || _apiSettings.Value.Offline.ToLowerInvariant() != "true")
            {
                tweets = GetAllTweets(tweetsToSkip, 12);
            }
            else
            {
                string fileName = "TestTweets.json";
                string jsonString = System.IO.File.ReadAllText(fileName);
                tweets = System.Text.Json.JsonSerializer.Deserialize<List<TweetModel>>(jsonString)!;
            }

            

            HttpContext.Session.SetInt32("NumberOfTweetsDisplayed", tweetsToSkip+12);

            return View("Tweets/LoadMoreTweets", tweets);
        }

        private List<TweetModel> GetAllTweets(int tweetsToSkip, int tweetsToReturn)
        {
            // You need to make your own API keys on on dev.twitter.com if you want to pull in LIVE tweets
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

