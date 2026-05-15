using H5YR.Core.Data.Interfaces;
using H5YR.Core.Models.ViewModels;
using H5YR.Core.Services;
using Microsoft.Extensions.Logging;

namespace H5YR.Core.Services
{
  public class CommunityStatsService : ICommunityStatsService
  {
    private readonly ILogger<CommunityStatsService> _logger;
    private readonly IPostCounterStore _postCounterStore;
    private readonly IWidgetH5yrStore _widgetH5yrStore;
    private readonly IMastodonService _mastodonService;

    public CommunityStatsService(
        ILogger<CommunityStatsService> logger,
        IPostCounterStore postCounterStore,
        IWidgetH5yrStore widgetH5yrStore,
        IMastodonService mastodonService)
    {
      _logger = logger;
      _postCounterStore = postCounterStore;
      _widgetH5yrStore = widgetH5yrStore;
      _mastodonService = mastodonService;
    }

    public async Task<CommunityStatsViewModel> GetStats()
    {
      var totalMastodon = _postCounterStore.GetPostCount();
      var totalWidget = _widgetH5yrStore.GetTotalCount();

      var postCounters = _postCounterStore.GetAll()
          .OrderBy(p => p.Date)
          .ToList();

      var activityTimeline = new List<ActivityDataPoint>();

      if (postCounters.Count > 0)
      {
        var monthlySnapshots = postCounters
            .GroupBy(p => new { p.Date.Year, p.Date.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new
            {
              Label = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
              g.OrderByDescending(x => x.Date).First().Quantity
            })
            .ToList();

        for (int i = 0; i < monthlySnapshots.Count; i++)
        {
          var count = i == 0
              ? monthlySnapshots[i].Quantity
              : monthlySnapshots[i].Quantity - monthlySnapshots[i - 1].Quantity;

          activityTimeline.Add(new ActivityDataPoint
          {
            Label = monthlySnapshots[i].Label,
            Count = Math.Max(0, count)
          });
        }
      }

      // Merge widget submissions into the timeline
      var allWidgets = _widgetH5yrStore.GetAll();
      var widgetByMonth = allWidgets
          .GroupBy(w => new { w.CreatedAt.Year, w.CreatedAt.Month })
          .ToDictionary(
              g => new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
              g => g.Count());

      foreach (var point in activityTimeline)
      {
        if (widgetByMonth.TryGetValue(point.Label, out var widgetCount))
        {
          point.Count += widgetCount;
          widgetByMonth.Remove(point.Label);
        }
      }

      foreach (var kvp in widgetByMonth.OrderBy(k => k.Key))
      {
        activityTimeline.Add(new ActivityDataPoint { Label = kvp.Key, Count = kvp.Value });
      }

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

      var todayStart = DateTime.UtcNow.Date;
      var todayCount = allWidgets.Count(w => w.CreatedAt >= todayStart);

      return new CommunityStatsViewModel
      {
        TotalH5yrs = totalMastodon + totalWidget,
        TotalWidgetSubmissions = totalWidget,
        TodayCount = todayCount,
        ActivityTimeline = activityTimeline,
        TopContributors = topContributors
      };
    }
  }
}
