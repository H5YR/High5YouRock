using h5yr.Core.Services;
using h5yr.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Json.Newtonsoft;
using Skybrud.Social.Mastodon.Models.Statuses;


namespace h5yr.Controllers.Mastodon
{

    [Route("api/loadmoreposts")]
    public class LoadMorePostsController : Controller
    {
        private readonly ILogger<LoadMorePostsController> _logger;
        private readonly IOptions<APISettings> _apiSettings;
        private readonly IMastodonService _mastodonService;

        protected bool IsOffline => _apiSettings.Value.Offline == "true";

        const string StartingPostId = "StartingPostId";
        const int PageSize = 12;


        public LoadMorePostsController(ILogger<LoadMorePostsController> logger, IOptions<APISettings> apiSettings, IMastodonService mastodonService)
        {
            _logger = logger;
            _apiSettings = apiSettings;
            _mastodonService = mastodonService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts(string startId = "")
        {
            // If we're passed in a starting ID, always start a new session otherwise returning users would start loading halfway through
            var startingPostId = string.IsNullOrEmpty(startId) ? HttpContext.Session.GetString(StartingPostId) : startId;

            IReadOnlyList<MastodonStatus> posts = Array.Empty<MastodonStatus>();

            if (IsOffline)
            {
                try
                {
                    string fileName = "TestStatuses.json";
                    posts = JsonUtils
                        .LoadJsonArray(fileName, MastodonStatus.Parse)
                        .SkipWhile(p => startingPostId == "" || p.Id != startingPostId)
                        .Skip(1)
                        .Take(PageSize)
                        .ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error: Unable to read test statuses JSON file");
                }
            }
            else
            {
                posts = await _mastodonService.GetStatuses(PageSize, startingPostId);
            }

            HttpContext.Session.SetString(StartingPostId, posts.LastOrDefault()?.Id ?? "");

            return View("Mastodon/LoadMorePosts", posts);
        }
    }
}

