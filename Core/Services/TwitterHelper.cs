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
        private readonly IOptions<APISettings> apiSettings;

        public TwitterHelper(ILogger<TwitterHelper> logger, IOptions<TwitterSettings> settings, IOptions<APISettings> apiSettings)
        {
            this.logger = logger;
            this.settings = settings;
            this.apiSettings = apiSettings;
        }

        public List<TweetModel> GetAllTweets(int tweetsToSkip, int tweetsToReturn)
        {

            List<TweetModel> Tweets = new List<TweetModel>();
            if (apiSettings.Value.Offline == null || apiSettings.Value?.Offline?.ToLowerInvariant() != "true")
            {
                var creds = new TwitterCredentials(settings.Value.ConsumerKey, settings.Value.ConsumerSecret, settings.Value.AccessToken, settings.Value.AccessTokenSecret);
                var userClient = new TwitterClient(creds);

                var searchResults = userClient.Search.SearchTweetsAsync("#h5yr");

                foreach (var tweet in searchResults.Result.Skip(tweetsToSkip).Take(tweetsToReturn))
                {
                    Tweets.Add(new TweetModel()
                    {

                        Username = tweet.CreatedBy.ToString(),
                        Avatar = tweet.CreatedBy.ProfileImageUrl,
                        Content = tweet.Text,
                        ScreenName = tweet.CreatedBy.ScreenName.ToString(),
                        TweetedOn = tweet.CreatedAt,
                        NumberOfTweets = Tweets.Count(),
                        ReplyToTweet = tweet.IdStr,
                        Url = tweet.Url

                    });

                };
            }

            return Tweets;
        }

    }
}