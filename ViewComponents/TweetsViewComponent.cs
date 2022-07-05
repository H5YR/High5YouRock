using h5yr.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tweetinvi;
using Tweetinvi.Models;

namespace h5yr.ViewComponents
{
    public class TweetsViewComponent : ViewComponent
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _accessToken;
        private readonly string _accessTokenSecret;

        public TweetsViewComponent(IOptions<TwitterSettings> twitterSettings)
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

        public IViewComponentResult Invoke(string loadmore = "false")
        {

            HttpContext.Session.SetInt32("NumberOfTweetsDisplayed", 12);

            List<TweetModel> tweets;

            tweets = GetAllTweets(0, 12);

            return View(tweets);
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

    }
}

