using h5yr.Settings;
using h5yr.ViewComponents;
using Microsoft.Extensions.Options;
using Tweetinvi;
using Tweetinvi.Models;

namespace H5YR.Core.Services
{
    public interface ITwitterHelper
    {
        List<TweetModel> GetAllTweets(int tweetsToSkip, int tweetsToReturn);
    }
    public class TwitterHelper : ITwitterHelper
    {
        private readonly ILogger<TwitterHelper> logger;
        private readonly IOptions<TwitterSettings> settings;

        public TwitterHelper(ILogger<TwitterHelper> logger, IOptions<TwitterSettings> settings)
        {
            this.logger = logger;
            this.settings = settings;
        }

        public List<TweetModel> GetAllTweets(int tweetsToSkip, int tweetsToReturn)
        {
            // You need to make sure your app on dev.twitter.com has read and write permissions if you wish to tweet!
            var creds = new TwitterCredentials(settings.Value.ConsumerKey, settings.Value.ConsumerSecret, settings.Value.AccessToken, settings.Value.AccessTokenSecret);
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