using H5YR.Core.Extensions;
using H5YR.Core.Models;
using H5YR.Core.Services;
using H5YR.Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Json.Newtonsoft;
using Skybrud.Social.Mastodon.Models.Statuses;

namespace H5YR.Core.ViewComponents
{
    public class MastodonViewComponent : ViewComponent
    {

        private readonly ILogger<MastodonViewComponent> _logger;
        private readonly IOptions<APISettings> _apiSettings;
        private readonly IMastodonService _mastodonService;
        private readonly IWidgetH5yrService _widgetH5yrService;

        protected bool IsOffline => _apiSettings.Value.Offline == "true";

        protected bool CreateOfflineFile => _apiSettings.Value.CreateOfflineFile == "true";

        public MastodonViewComponent(
            ILogger<MastodonViewComponent> logger,
            IOptions<APISettings> apiSettings,
            IMastodonService mastodonService,
            IWidgetH5yrService widgetH5yrService)
        {
            _logger = logger;
            _apiSettings = apiSettings;
            _mastodonService = mastodonService;
            _widgetH5yrService = widgetH5yrService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            // Get the Mastodon statuses
            IReadOnlyList<MastodonStatus> statuses = await GetStatuses();

            // Get the total from local storage (Mastodon + widget)
            int mastodonCount = _mastodonService.GetPostCount();
            int widgetCount = _widgetH5yrService.GetTotalCount();
            int totalCount = mastodonCount + widgetCount;

            // Build unified feed: merge Mastodon posts + widget h5yrs sorted by date
            var feedItems = BuildUnifiedFeed(statuses);

            // Initialize a new model for the view component
            MastodonModel model = new(statuses, totalCount, feedItems);

            // Return the view
            return View(model);

        }

        private IReadOnlyList<FeedItem> BuildUnifiedFeed(IReadOnlyList<MastodonStatus> mastodonStatuses)
        {
            // Convert Mastodon statuses to FeedItems
            var mastodonItems = mastodonStatuses.Select(s => new FeedItem
            {
                Id = $"mastodon_{s.Id}",
                Source = "mastodon",
                AvatarUrl = s.Account.Avatar,
                Username = s.Account.Username,
                ProfileUrl = s.Account.Url,
                ContentHtml = s.Content.ReplaceCustomEmojis(s.Emojis),
                Permalink = s.Url,
                // Use UtcDateTime so DateTimeKind is Utc — ensures .ToString("o") emits a Z suffix for timeago
                CreatedAt = s.CreatedAt.DateTimeOffset.UtcDateTime
            });

            // Get recent widget h5yr items (same count as Mastodon to have a good mix)
            var widgetItems = _widgetH5yrService.GetRecentFeedItems(12);

            // Merge and sort by date descending
            return mastodonItems
                .Concat(widgetItems)
                .OrderByDescending(f => f.CreatedAt)
                .ToList();
        }

        private async Task<IReadOnlyList<MastodonStatus>> GetStatuses()
        {

            string fileName = "TestStatuses.json";

            if (IsOffline)
            {
                try
                {
                    return JsonUtils.LoadJsonArray(fileName, MastodonStatus.Parse).Take(12).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error: Unable to read test statuses JSON file");
                    return Array.Empty<MastodonStatus>();
                }
            }

            IReadOnlyList<MastodonStatus> statuses = await _mastodonService.GetStatuses(12);

            if (CreateOfflineFile)
            {
                statuses = await _mastodonService.GetStatuses(36); // If creating an offline file, get a bigger initial pull of posts to aid testing 'load more', but then discard some.
                try
                {
                    JsonUtils.SaveJsonArray(fileName, statuses);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error: Unable to write test statuses JSON file");
                }
            }

            return statuses.Take(12).ToList();

        }

    }
}
