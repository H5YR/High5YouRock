using H5YR.Core.Models;
using Microsoft.Extensions.Logging;

namespace H5YR.Core.Services
{
    public class UnifiedFeedService : IUnifiedFeedService
    {
        private readonly ILogger<UnifiedFeedService> _logger;
        private readonly IMastodonService _mastodonService;
        private readonly IDiscourseService _discourseService;

        public UnifiedFeedService(
            ILogger<UnifiedFeedService> logger,
            IMastodonService mastodonService,
            IDiscourseService discourseService)
        {
            _logger = logger;
            _mastodonService = mastodonService;
            _discourseService = discourseService;
        }

        public async Task<IEnumerable<UnifiedFeedItem>> GetUnifiedFeedAsync(int count = 12)
        {
            try
            {
                // Fetch more than needed from each source to ensure we have enough after merging
                var mastodonItems = _mastodonService.GetStatusesAsFeedItems(count);
                var discourseItems = _discourseService.GetRecentReactionsAsFeedItems(count);

                // Merge and sort by date (newest first)
                var unifiedFeed = mastodonItems
                    .Concat(discourseItems)
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
            return _mastodonService.GetPostCount() + _discourseService.GetTotalReactionCount();
        }
    }
}