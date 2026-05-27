using H5YR.Core.Data.Entities;
using H5YR.Core.Models;

namespace H5YR.Core.Services
{
    public interface IWidgetH5yrService
    {
        /// <summary>
        /// Get recent widget h5yr submissions as FeedItems.
        /// </summary>
        IEnumerable<FeedItem> GetRecentFeedItems(int count);

        /// <summary>
        /// Get widget h5yr submissions older than the given date as FeedItems.
        /// </summary>
        IEnumerable<FeedItem> GetFeedItemsBefore(DateTime before, int count);

        /// <summary>
        /// Submit a new h5yr from the widget. Returns the created entity or null if rate limited.
        /// </summary>
        WidgetH5yr? Submit(string authProvider, string externalUserId, string displayName,
            string avatarUrl, string profileUrl, string targetUrl);

        /// <summary>
        /// Get remaining submissions for a user on a specific site today.
        /// </summary>
        int GetRemainingSubmissions(string externalUserId, string authProvider, string targetSiteHost);

        /// <summary>
        /// Get total widget h5yr count.
        /// </summary>
        int GetTotalCount();
    }
}
