using H5YR.Core.Data.Entities;
using H5YR.Core.Data.Interfaces;
using H5YR.Core.Models;
using Microsoft.Extensions.Logging;

namespace H5YR.Core.Services
{
    public class UnifiedFeedService : IUnifiedFeedService
    {
        private readonly ILogger<UnifiedFeedService> _logger;
        private readonly IMastodonService _mastodonService;
        private readonly IDiscourseService _discourseService;
        private readonly IWidgetH5yrService _widgetService;
        private readonly IFeedItemLogStore _feedItemLogStore;

        public UnifiedFeedService(
            ILogger<UnifiedFeedService> logger,
            IMastodonService mastodonService,
            IDiscourseService discourseService,
            IWidgetH5yrService widgetService,
            IFeedItemLogStore feedItemLogStore)
        {
            _logger = logger;
            _mastodonService = mastodonService;
            _discourseService = discourseService;
            _widgetService = widgetService;
            _feedItemLogStore = feedItemLogStore;
        }

        public async Task<IEnumerable<UnifiedFeedItem>> GetUnifiedFeedAsync(int count = 12)
        {
            try
            {
                // Fetch more than needed from each source to ensure we have enough after merging
                var mastodonItems = _mastodonService.GetStatusesAsFeedItems(count).ToList();
                var discourseItems = _discourseService.GetRecentReactionsAsFeedItems(count);
                var widgetItems = _widgetService.GetRecentFeedItems(count)
                    .Select(w => new UnifiedFeedItem
                    {
                        Id = w.Id,
                        SourceType = FeedSourceType.Widget,
                        CreatedAt = w.CreatedAt,
                        UserName = w.Username,
                        UserDisplayName = w.Username,
                        UserAvatar = w.AvatarUrl,
                        Content = w.ContentHtml,
                        RawContent = w.ContentHtml,
                        Url = w.Permalink
                    })
                    .ToList();

                // Record new Mastodon and widget items in the durable log used for date-based
                // community stats. This runs on the live feed path (previously it lived in the
                // now-unused MastodonViewComponent, which left the stats frozen).
                LogNewFeedItems(mastodonItems);
                LogNewFeedItems(widgetItems);

                // Merge and sort by date (newest first)
                var unifiedFeed = mastodonItems
                    .Concat(discourseItems)
                    .Concat(widgetItems)
                    .OrderByDescending(item => item.CreatedAt)
                    .Take(count)
                    .ToList();

                return await Task.FromResult(unifiedFeed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating unified feed");
                return Enumerable.Empty<UnifiedFeedItem>();
            }
        }

        public int GetTotalCount()
        {
            return _mastodonService.GetPostCount() + _discourseService.GetTotalReactionCount() + _widgetService.GetTotalCount();
        }

        /// <summary>
        /// Persists any new feed items to the log table so date-based stats
        /// (activity timeline, today count) stay current. Only Mastodon and widget
        /// items are recorded, matching the original stats behaviour; Discourse
        /// reactions are shown in the feed but excluded from the counts.
        /// </summary>
        private void LogNewFeedItems(IEnumerable<UnifiedFeedItem> items)
        {
            foreach (var item in items)
            {
                // Build the canonical ExternalId per source so we de-duplicate correctly
                // against the historical back-fill (which used the "mastodon_"/"widget_" prefixes).
                // Widget items already carry the "widget_{id}" prefix; Mastodon items carry the raw id.
                string? externalId = item.SourceType switch
                {
                    FeedSourceType.Mastodon => $"mastodon_{item.Id}",
                    FeedSourceType.Widget => item.Id,
                    _ => null
                };

                if (externalId is null)
                {
                    continue;
                }

                try
                {
                    if (!_feedItemLogStore.Exists(externalId))
                    {
                        _feedItemLogStore.Save(new FeedItemLog
                        {
                            ExternalId = externalId,
                            Source = item.SourceType == FeedSourceType.Mastodon ? "mastodon" : "widget",
                            CreatedAt = item.CreatedAt
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error logging feed item {ExternalId}", externalId);
                }
            }
        }
    }
}