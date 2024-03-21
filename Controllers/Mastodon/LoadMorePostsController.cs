using h5yr.Settings;
using h5yr.ViewComponents;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tweetinvi;
using Tweetinvi.Models;


namespace h5yr.Controllers.Mastodon
{

    [Route("api/loadmoreposts")]
    public class LoadMorePostsController : Controller
    {
        private readonly ILogger<LoadMorePostsController> _logger;
        private readonly IOptions<APISettings> _apiSettings;

        const string NumberOfPostsDisplayed = "NumberOfPostsDisplayed";


        public LoadMorePostsController(IOptions<TwitterSettings> twitterSettings, ILogger<LoadMorePostsController> logger, IOptions<APISettings> apiSettings)
        {
            _logger = logger;
            _apiSettings = apiSettings;
        }

        [HttpGet]
        public IActionResult GetPosts()
        {
            var postsToSkip = Convert.ToInt32(HttpContext.Session.GetInt32(NumberOfPostsDisplayed));
            postsToSkip = postsToSkip == 0 ? 12 : postsToSkip;

            List<MastodonModel> posts = new();

            if (_apiSettings.Value.Offline == null || _apiSettings.Value.Offline.ToLowerInvariant() != "true")
            {
                posts = GetAllPosts(postsToSkip, 12);
            }
            else
            {
                string fileName = "TestTweets.json";
                string jsonString = System.IO.File.ReadAllText(fileName);
                posts = System.Text.Json.JsonSerializer.Deserialize<List<MastodonModel>>(jsonString)!;
            }



            HttpContext.Session.SetInt32(NumberOfPostsDisplayed, postsToSkip + 12);

            return View("Mastodon/LoadMorePosts", posts);
        }

        private List<MastodonModel> GetAllPosts(int postsToSkip, int postsToReturn)
        {
            // You need to make your own API keys on on dev.twitter.com if you want to pull in LIVE tweets
            // TODO - complete replacement of Twitter implementation - stub for now
            var creds = new TwitterCredentials("", "", "", "");
            var userClient = new TwitterClient(creds);

            var searchResults = userClient.Search.SearchTweetsAsync("#h5yr");


            List<MastodonModel> FetchPosts = new List<MastodonModel>();


            foreach (var post in searchResults.Result.Skip(postsToSkip).Take(postsToReturn))
            {


            };

            return FetchPosts;
        }
        public List<MastodonModel> GetMorePosts()
        {

            var postsToSkip = Convert.ToInt32(HttpContext.Session.GetInt32(NumberOfPostsDisplayed));

            List<MastodonModel> posts = GetAllPosts(postsToSkip, 12);

            HttpContext.Session.SetInt32(NumberOfPostsDisplayed, postsToSkip + 12);

            return posts;
        }
    }
}

