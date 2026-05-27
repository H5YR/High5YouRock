using H5YR.Core.Data.Interfaces;
using H5YR.Core.Models.ViewModels;
using H5YR.Core.Services;
using Microsoft.Extensions.Logging;

namespace H5YR.Core.Services
{
  public class CommunityStatsService : ICommunityStatsService
  {
    private readonly ILogger<CommunityStatsService> _logger;
    private readonly IFeedItemLogStore _feedItemLogStore;
    private readonly IWidgetH5yrStore _widgetH5yrStore;
    private readonly IMastodonService _mastodonService;

    public CommunityStatsService(
        ILogger<CommunityStatsService> logger,
        IFeedItemLogStore feedItemLogStore,
        IWidgetH5yrStore widgetH5yrStore,
        IMastodonService mastodonService)
    {
      _logger = logger;
      _feedItemLogStore = feedItemLogStore;
      _widgetH5yrStore = widgetH5yrStore;
      _mastodonService = mastodonService;
    }

    public async Task<CommunityStatsViewModel> GetStats()
    {
      // Use FeedItemLog as the single source of truth for all counts and timeline data
      var allLogItems = _feedItemLogStore.GetAll().ToList();

      var totalH5yrs = allLogItems.Count;
      var totalWidget = allLogItems.Count(x => x.Source == "widget");

      // Build activity timeline by grouping log items by month, sorted chronologically
      var activityTimeline = allLogItems
          .GroupBy(x => new DateTime(x.CreatedAt.Year, x.CreatedAt.Month, 1))
          .OrderBy(g => g.Key)
          .Select(g => new ActivityDataPoint
          {
            Label = g.Key.ToString("MMM yyyy"),
            Count = g.Count()
          })
          .ToList();

      var todayStart = DateTime.UtcNow.Date;
      var todayCount = allLogItems.Count(x => x.CreatedAt >= todayStart);

      var topContributors = new List<ContributorDataPoint>();

      // Get Mastodon authors from cached feed
      try
      {
        var mastodonPosts = await _mastodonService.GetStatuses(40);
        var mastodonContributors = mastodonPosts
            .GroupBy(p => p.Account.Username)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => new ContributorDataPoint
            {
              DisplayName = g.Key,
              AvatarUrl = g.First().Account.Avatar,
              ProfileUrl = g.First().Account.Url,
              Count = g.Count()
            });
        topContributors.AddRange(mastodonContributors);
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex, "Failed to fetch Mastodon posts for stats");
      }

      var allWidgets = _widgetH5yrStore.GetAll();
      var widgetContributors = allWidgets
          .GroupBy(w => new { w.ExternalUserId, w.AuthProvider })
          .OrderByDescending(g => g.Count())
          .Take(10)
          .Select(g =>
          {
            var latest = g.OrderByDescending(x => x.CreatedAt).First();
            return new ContributorDataPoint
            {
              DisplayName = latest.DisplayName,
              AvatarUrl = latest.AvatarUrl,
              ProfileUrl = latest.ProfileUrl,
              Count = g.Count()
            };
          });
      topContributors.AddRange(widgetContributors);

      topContributors = topContributors
          .OrderByDescending(c => c.Count)
          .Take(10)
          .ToList();

      return new CommunityStatsViewModel
      {
        TotalH5yrs = totalH5yrs,
        TotalWidgetSubmissions = totalWidget,
        TodayCount = todayCount,
        ActivityTimeline = activityTimeline,
        TopContributors = topContributors
      };
    }
  }
}
