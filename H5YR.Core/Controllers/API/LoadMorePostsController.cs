using H5YR.Core.Extensions;
using H5YR.Core.Models;
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
        private readonly IWidgetH5yrService _widgetH5yrService;

        protected bool IsOffline => _apiSettings.Value.Offline == "true";

        const string StartingPostId = "StartingPostId";
        const int PageSize = 12;


        public LoadMorePostsController(
            ILogger<LoadMorePostsController> logger,
            IOptions<APISettings> apiSettings,
            IMastodonService mastodonService,
            IWidgetH5yrService widgetH5yrService)
        {
            _logger = logger;
            _apiSettings = apiSettings;
            _mastodonService = mastodonService;
            _widgetH5yrService = widgetH5yrService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts(string startId = "", string oldestDate = "")
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

            // Convert Mastodon posts to FeedItems
            var mastodonFeedItems = posts.Select(s => new FeedItem
            {
                Id = $"mastodon_{s.Id}",
                Source = "mastodon",
                AvatarUrl = s.Account.Avatar,
                Username = s.Account.Username,
                ProfileUrl = s.Account.Url,
                ContentHtml = s.Content.ReplaceCustomEmojis(s.Emojis),
                Permalink = s.Url,
                CreatedAt = s.CreatedAt.DateTimeOffset.DateTime
            }).ToList();

            // Get widget h5yr items from before the oldest date shown
            var widgetItems = new List<FeedItem>();
            if (DateTime.TryParse(oldestDate, out var beforeDate))
            {
                widgetItems = _widgetH5yrService.GetFeedItemsBefore(beforeDate, PageSize).ToList();
            }

            // Merge and sort
            var allItems = mastodonFeedItems
                .Concat(widgetItems)
                .OrderByDescending(f => f.CreatedAt)
                .ToList();

            return View("Mastodon/LoadMorePosts", allItems);
        }
    }
}
