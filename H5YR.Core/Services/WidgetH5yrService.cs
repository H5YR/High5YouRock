using System.Web;
using H5YR.Core.Data.Entities;
using H5YR.Core.Data.Interfaces;
using H5YR.Core.Models;
using H5YR.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace H5YR.Core.Services
{
    public class WidgetH5yrService : IWidgetH5yrService
    {
        private readonly ILogger<WidgetH5yrService> _logger;
        private readonly IWidgetH5yrStore _store;
        private readonly IOptions<WidgetSettings> _widgetSettings;

        public WidgetH5yrService(
            ILogger<WidgetH5yrService> logger,
            IWidgetH5yrStore store,
            IOptions<WidgetSettings> widgetSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _widgetSettings = widgetSettings ?? throw new ArgumentNullException(nameof(widgetSettings));
        }

        public IEnumerable<FeedItem> GetRecentFeedItems(int count)
        {
            return _store.GetRecent(count).Select(MapToFeedItem);
        }

        public IEnumerable<FeedItem> GetFeedItemsBefore(DateTime before, int count)
        {
            return _store.GetRecentBefore(before, count).Select(MapToFeedItem);
        }

        public WidgetH5yr? Submit(string authProvider, string externalUserId, string displayName,
            string avatarUrl, string profileUrl, string targetUrl)
        {
            // Validate URL
            if (!Uri.TryCreate(targetUrl, UriKind.Absolute, out var uri) ||
                (uri.Scheme != "http" && uri.Scheme != "https"))
            {
                _logger.LogWarning("Invalid target URL submitted: {TargetUrl}", targetUrl);
                return null;
            }

            var targetSiteHost = uri.Host.ToLowerInvariant();
            var rateLimit = _widgetSettings.Value.RateLimitPerSitePerDay;

            // Check rate limit
            var todayCount = _store.GetSubmissionCountToday(externalUserId, authProvider, targetSiteHost);
            if (todayCount >= rateLimit)
            {
                _logger.LogInformation(
                    "Rate limit reached for user {UserId} on site {Host}. {Count}/{Limit} today.",
                    externalUserId, targetSiteHost, todayCount, rateLimit);
                return null;
            }

            // Sanitize the display URL for content
            var displayUrl = HttpUtility.HtmlEncode(targetUrl);

            var entity = new WidgetH5yr
            {
                AuthProvider = authProvider,
                ExternalUserId = externalUserId,
                DisplayName = displayName,
                AvatarUrl = avatarUrl,
                ProfileUrl = profileUrl,
                TargetUrl = targetUrl,
                TargetSiteHost = targetSiteHost,
                Content = $"<p><span class=\"h5yr\">#H5YR</span> for <a href=\"{displayUrl}\" target=\"_blank\" rel=\"noopener\">{displayUrl}</a></p>",
                CreatedAt = DateTime.UtcNow
            };

            _store.Save(entity);

            _logger.LogInformation(
                "Widget h5yr submitted by {DisplayName} ({Provider}) for {TargetUrl}",
                displayName, authProvider, targetUrl);

            return entity;
        }

        public int GetRemainingSubmissions(string externalUserId, string authProvider, string targetSiteHost)
        {
            var rateLimit = _widgetSettings.Value.RateLimitPerSitePerDay;
            var todayCount = _store.GetSubmissionCountToday(externalUserId, authProvider, targetSiteHost);
            return Math.Max(0, rateLimit - todayCount);
        }

        public int GetTotalCount()
        {
            return _store.GetTotalCount();
        }

        private static FeedItem MapToFeedItem(WidgetH5yr entity)
        {
            return new FeedItem
            {
                Id = $"widget_{entity.Id}",
                Source = "widget",
                AvatarUrl = entity.AvatarUrl,
                Username = entity.DisplayName,
                ProfileUrl = entity.ProfileUrl,
                ContentHtml = entity.Content,
                Permalink = entity.TargetUrl,
                CreatedAt = entity.CreatedAt,
                AuthProvider = entity.AuthProvider
            };
        }
    }
}
