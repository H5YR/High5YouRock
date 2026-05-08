using H5YR.Core.Services;
using H5YR.Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Json.Newtonsoft;
using Skybrud.Social.Mastodon.Models.Statuses;

namespace H5YR.Core.Controllers.API
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
            var startingPostId = string.IsNullOrEmpty(startId) ? HttpContext.Request.Cookies[StartingPostId] : startId;

            startingPostId = long.TryParse(startingPostId, out long parsedResult) ? parsedResult.ToString() : ""; // Extra id tamper check

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

            HttpContext.Response.Cookies.Append(StartingPostId, posts.LastOrDefault()?.Id ?? "");

            return View("Mastodon/LoadMorePosts", posts);
        }
    }
}
