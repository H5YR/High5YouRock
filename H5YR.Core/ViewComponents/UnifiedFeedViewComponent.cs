using H5YR.Core.Models;
using H5YR.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace H5YR.Core.ViewComponents
{
    public class UnifiedFeedViewComponent : ViewComponent
    {
        private readonly ILogger<UnifiedFeedViewComponent> _logger;
        private readonly IUnifiedFeedService _unifiedFeedService;

        public UnifiedFeedViewComponent(
            ILogger<UnifiedFeedViewComponent> logger,
            IUnifiedFeedService unifiedFeedService)
        {
            _logger = logger;
            _unifiedFeedService = unifiedFeedService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int count = 12)
        {
            var feedItems = await _unifiedFeedService.GetUnifiedFeedAsync(count);
            var totalCount = _unifiedFeedService.GetTotalCount();

            var model = new UnifiedFeedModel
            {
                FeedItems = feedItems.ToList(),
                TotalCount = totalCount
            };

            return View(model);
        }
    }

    public class UnifiedFeedModel
    {
        public List<UnifiedFeedItem> FeedItems { get; set; } = new();
        public int TotalCount { get; set; }
    }
}